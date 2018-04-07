using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbitFind
{
    class TreeNode
    {
        public string attribute { get; set; }= "";
        public List<TreeNode> children { get; set;} = new List<TreeNode>();
        public string id { get; set; } = "";
        public string text { get; set; } = "";
        public List<string> special { get; set; } = new List<string>();

        public TreeNode() { }

        /*hopefully it works*/
        public string toString() {
            return "<"+attribute+" "+id+" "+special.ToString()+">"+"\t"+children.ToString()+"\t"+text+"</"+attribute+">";
        }
    }
}
