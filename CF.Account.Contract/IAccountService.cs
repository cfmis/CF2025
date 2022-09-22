using System;
using System.Collections.Generic;

namespace CF.Account.Contract
{
    public interface IAccountService
    {
        LoginInfo GetLoginInfo(Guid token);
        LoginInfo Login(string loginName, string password,string languageid);
        void Logout(Guid token);
        void ModifyPwd(t_User user);

        t_User GetUser(int id);
        IEnumerable<t_User> GetUserList(UserRequest request = null);
        void SaveUser(t_User user);
        void DeleteUser(List<int> ids);

        Role GetRole(int id);
        IEnumerable<Role> GetRoleList(RoleRequest request = null);
        void SaveRole(Role role);
        void DeleteRole(List<int> ids);

        Guid SaveVerifyCode(string verifyCodeText);
        bool CheckVerifyCode(string verifyCodeText, Guid guid);
    }
}
