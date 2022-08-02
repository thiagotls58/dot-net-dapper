using System.Collections.Generic;
using System.Linq;
using Blog.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Blog.Repositories
{
    public class UserRepository : Repository<User>
    {
        public UserRepository(SqlConnection connection) : base(connection)
        {
        }

        public List<User> GetWithRoles()
        {
            var query = @"
                select
                    [User].*,
                    [Role].*
                from
                    [User]
                    left join [UserRole] on [User].[Id] = [UserRole].[UserId]
                    left join [Role] on [UserRole].[RoleId] = [Role].[Id]
            ";

            var users = new List<User>();

            var items = _connection.Query<User, Role, User>(
                query,
                (user, role) => // mapeamento: on to many
                {
                    var usr = users.FirstOrDefault(x => x.Id == user.Id);
                    if (usr == null)
                    {
                        usr = user;
                        if (role != null)
                            usr.Roles.Add(role);
                        
                        users.Add(usr);
                    }
                    else 
                    {
                        if (role != null)
                            usr.Roles.Add(role);
                    }
                    return user;
                }, splitOn: "Id");

            return users;
        }
    }
}