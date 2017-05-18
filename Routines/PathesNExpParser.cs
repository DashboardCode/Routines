using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Vse.Routines
{
    public class PathesNExpParser<TRootEntity> : INExpParser<TRootEntity>
    {
        public readonly List<string[]> Pathes = new List<string[]>();
        private string[] sequence;
        public void ParseRoot<TEntity>(Expression<Func<TRootEntity, TEntity>> navigationExpression)
        {
            var name = MemberExpressionExtensions.GetMemberName(navigationExpression);
            sequence = Add(new string[] { }, name);
        }
        public void ParseRootEnumerable<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            ParseRoot(navigationExpression);
        }
        public void Parse<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity>> navigationExpression)
        {
            var name = MemberExpressionExtensions.GetMemberName(navigationExpression);
            sequence = Add(sequence.ToArray(), name);
        }
        public void ParseEnumerable<TThenEntity, TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> navigationExpression)
        {
            Parse(navigationExpression);
        }
        private string[] Add(string[] parentPath, string member)
        {
            var newPath = parentPath.Concat(new[] { member }).ToArray();
            var subpahtes = new List<string[]>();
            foreach (var p in Pathes)
            {
                var isSub = true;
                for (int i = 0; i <= parentPath.Length; i++)
                {
                    if (i == parentPath.Length)
                    {
                        if (p.Length > i && p[i] == member) // there is full subpath
                        {
                            goto end;
                        }
                    }
                    else
                    {
                        if (parentPath.Length < i || p[i] != parentPath[i])
                        {
                            isSub = false;
                            break;
                        }
                    }
                }
                if (isSub)
                    subpahtes.Add(p);
            }
            if (subpahtes.Count > 0)
            {
                var root = Pathes.Where(e => e.SequenceEqual(parentPath)).FirstOrDefault();
                if (root != null)
                    Pathes.Remove(root);
            }
            Pathes.Add(newPath);
            end:
            return newPath;
        }
    }
}
