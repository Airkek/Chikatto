using System;

namespace Chikatto.Enums
{
    //author: cmyui https://github.com/cmyui/gulag/blob/master/constants/privileges.py
    
    [Flags]
    public enum Privileges
    {
        Public = 1,
        Normal = 2 << 0,
        Donor = 2 << 1,
        AdminAccessRap = 2 << 2,
        AdminManageUsers = 2 << 3,
        AdminBanUsers = 2 << 4,
        AdminSilenceUsers = 2 << 5,
        AdminWipeUsers = 2 << 6,
        AdminManageBeatmaps = 2 << 7,
        AdminManageServers = 2 << 8,
        AdminManageSettings = 2 << 9,
        AdminManageBetakeys = 2 << 10,
        AdminManageReports = 2 << 11,
        AdminManageDocs = 2 << 12,
        AdminManageBadges = 2 << 13,
        AdminViewRapLogs = 2 << 14,
        AdminManagePrivileges = 2 << 15,
        AdminSendAlerts = 2 << 16,
        AdminChatMod = 2 << 17,
        AdminKickUsers = 2 << 18,
        PendingVerification = 2 << 19,
        TournamentStaff = 2 << 20,
        AdminCaker = 2 << 21,
        Restricted = 2 << 22,
        
        Nominator = AdminViewRapLogs | AdminManageBeatmaps,
        Mod = AdminViewRapLogs | AdminManageBetakeys | AdminManageUsers | AdminChatMod | AdminSendAlerts | AdminSilenceUsers | AdminManageReports,
        Admin = Mod | AdminKickUsers | AdminBanUsers | AdminManageBadges | AdminWipeUsers,
        Owner = Nominator | Admin | AdminAccessRap | AdminManageServers | AdminManageSettings | AdminManageDocs | AdminManagePrivileges | TournamentStaff | AdminCaker,
        Staff = Owner
    }
}