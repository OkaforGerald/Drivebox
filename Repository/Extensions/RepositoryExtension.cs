using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Repository.Extensions
{
    public static class RepositoryExtension
    {
        public static IQueryable<T> Sort<T>(this IQueryable<T> source, string OrderQuery)
        {
            if (string.IsNullOrWhiteSpace(OrderQuery)) return source;

            StringBuilder sb = new StringBuilder();
            var props = OrderQuery.Split(',');

            foreach(var prop in props)
            {
                var query = prop.Trim();
                var property = prop.Split(" ")[0];

                var TProperties = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                var propInfo = TProperties.FirstOrDefault(x => x.Name == property);

                if (propInfo is not null)
                {
                    string parameter = $"{propInfo.Name} {prop.Split(' ')[1].Trim() switch { "asc" => "ascending",
                        "desc" => "descending",
                        _ => null }},";

                    sb.Append(parameter);
                }
            }
            return source.OrderBy(sb.ToString().TrimEnd(',', ' '));
        }

        public static IQueryable<Folder> Search(this IQueryable<Folder> source, string searchTerm)
        {
            if(string.IsNullOrWhiteSpace(searchTerm)) return source;

            return source.Where(x => x.Name.ToLower().Contains(searchTerm.ToLower()));
        }

        public static IQueryable<Content> Search(this IQueryable<Content> source, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return source;

            return source.Where(x => x.Name.ToLower().Contains(searchTerm.ToLower()));
        }

        public static IQueryable<Folder> Filter(this IQueryable<Folder> source, string FolderType)
        {
            if (string.IsNullOrEmpty(FolderType)) return source;

            var access = FolderType switch
            {
                "Public" => Access.Public,
                _ => Access.Private
            };

            return source.Where(x => x.Access.Equals(access));
        }
    }
}
