using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApps.DeviceList
{
    /// <summary>
    ///  ツリー構造.
    /// </summary>
    /// <typeparam name="T">ツリーに格納されるデータの型</typeparam>
    public class TreeNode<T>
    {
        public T Data { get; set; }

        /// <summary>
        ///  ツリーの親.
        /// </summary>
        public TreeNode<T> Parent { get; set; }

        /// <summary>
        ///  ツリーの子.
        /// </summary>
        public List<TreeNode<T>> Children { get; set; }

        /// <summary>
        ///  親がいるかどうか.
        /// </summary>
        public System.Boolean HasParent
        {
            get
            {
                return this.Parent != null;
            }
        }

        /// <summary>
        ///  ルートかどうか.
        /// </summary>
        public System.Boolean IsRoot
        {
            get
            {
                return this.Parent == null;
            }
        }

        /// <summary>
        ///  子がいるかどうか.
        /// </summary>
        public System.Boolean HasChildren
        {
            get
            {
                return (Children != null) && (Children.Count > 0);
            }
        }

        /// <summary>
        ///  深さ.
        /// </summary>
        public System.Int32 Depth
        {
            get
            {
                int depth = 0;
                var node = this;
                while (node.HasParent)
                {
                    ++depth;
                    node = node.Parent;
                }

                return depth;
            }
        }

        /// <summary>
        ///  指定した階数だけ親のアイテムを取得する.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public TreeNode<T> GetAncestor(int level)
        {
            if (level < 0)
            {
                throw new ArgumentOutOfRangeException("level");
            }

            var ancestor = this;
            for (int i = 0; i < level; ++i)
            {
                ancestor = ancestor.Parent;
                if (ancestor == null)
                {
                    break;
                }
            }

            return ancestor;
        }
    }
}
