using CollegeManagementWPF.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace CollegeManagementWPF
{
    /// <summary>
    /// Tracks the logged-in admin's identity, role, permissions and department scope.
    /// Loaded at login, cleared on sign-out.
    /// </summary>
    public static class SessionUser
    {
        public static string Username { get; private set; } = "";
        public static int    AdminId  { get; private set; } = -1;
        public static int    RoleId   { get; private set; } = -1;
        public static string RoleName { get; private set; } = "";

        // Super Admin = role name is "Super Admin" OR no role row found in roles table
        // (legacy accounts where priority didn't match any role)
        public static bool IsSuperAdmin { get; private set; } = true;

        private static readonly HashSet<string> _perms = new(StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> _depts = new(StringComparer.OrdinalIgnoreCase);

        public static IReadOnlySet<string> Permissions  => _perms;
        public static IReadOnlySet<string> AllowedDepts => _depts;

        /// <summary>True when no department restriction is set — can access all depts.</summary>
        public static bool AllDepts => _depts.Count == 0;

        /// <summary>Returns true if this user has the given permission key.</summary>
        public static bool Has(string permissionKey)
        {
            if (IsSuperAdmin) return true;
            return _perms.Contains(permissionKey);
        }

        public static bool CanAccessDept(string deptId)
        {
            if (IsSuperAdmin || AllDepts) return true;
            return _depts.Contains(deptId);
        }

        /// <summary>
        /// Loads session data from DB after successful login.
        /// </summary>
        public static void Load(string username, int adminId, int roleId, DBConnect db)
        {
            Username     = username;
            AdminId      = adminId;
            RoleId       = roleId;
            RoleName     = "";
            IsSuperAdmin = true; // default: unrestricted until we confirm a role exists
            _perms.Clear();
            _depts.Clear();

            try
            {
                var conn = db.GetConnection();
                conn.Open();

                // Look up the role name from the roles table using priority as role_id
                string? foundRole = null;
                if (roleId > 0)
                {
                    using var cmdRole = new MySqlCommand(
                        "SELECT role_name FROM ecc_dof_wukrostmarycollege.roles WHERE role_id=@r LIMIT 1", conn);
                    cmdRole.Parameters.AddWithValue("@r", roleId);
                    foundRole = cmdRole.ExecuteScalar()?.ToString()?.Trim();
                }

                if (!string.IsNullOrEmpty(foundRole))
                {
                    // A role row exists — enforce permissions
                    RoleName     = foundRole;
                    IsSuperAdmin = foundRole.Equals("Super Admin", StringComparison.OrdinalIgnoreCase);

                    if (!IsSuperAdmin)
                    {
                        // Load allowed permission keys
                        using (var cmdPerms = new MySqlCommand(
                            "SELECT permission_key FROM ecc_dof_wukrostmarycollege.role_permissions " +
                            "WHERE role_id=@r AND is_allowed=1", conn))
                        {
                            cmdPerms.Parameters.AddWithValue("@r", roleId);
                            using var rp = cmdPerms.ExecuteReader();
                            while (rp.Read())
                                _perms.Add(rp[0]?.ToString() ?? "");
                        }

                        // Load department scope
                        try
                        {
                            using var cmdDept = new MySqlCommand(
                                "SELECT dept_id FROM ecc_dof_wukrostmarycollege.role_dept_scope " +
                                "WHERE role_id=@r", conn);
                            cmdDept.Parameters.AddWithValue("@r", roleId);
                            using var rd = cmdDept.ExecuteReader();
                            while (rd.Read())
                                _depts.Add(rd[0]?.ToString() ?? "");
                        }
                        catch { /* table not yet created — no dept restriction */ }
                    }
                }
                else
                {
                    // No matching role in roles table → legacy/unassigned account → full access
                    RoleName     = "Admin";
                    IsSuperAdmin = true;
                }

                conn.Close();
            }
            catch
            {
                // DB offline → grant full access so the app is usable
                RoleName     = "Admin";
                IsSuperAdmin = true;
            }
        }

        public static void Clear()
        {
            Username     = "";
            AdminId      = -1;
            RoleId       = -1;
            RoleName     = "";
            IsSuperAdmin = true;
            _perms.Clear();
            _depts.Clear();
        }
    }
}
