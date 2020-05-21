using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace DefaultPanel
{
    public class DefaultPanelInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "DefaultPanel";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "Defaults panels by Mathias Sønderskov and Long Nguyen 2020";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("f52aae61-7ad5-49ba-8ff1-36da98c9cca1");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Mathias Sønderskov and Long Nguyen";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
