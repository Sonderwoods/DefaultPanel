using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DefaultPanel
{
    public partial class GhcDefaultPanel : GH_Component
    {

        private Guid targetPanelComponentGuid = Guid.Empty;
        public Guid TargetPanelComponentGuid {
            get
            {
                return targetPanelComponentGuid;
            }
            set
            {
                targetPanelComponentGuid = value;
            }
         }

        /// <summary>
        /// Initializes a new instance of the GhcDefaultPanelTest class.
        /// </summary>
        public GhcDefaultPanel()
             : base(
                 "Default",
                 "▼",
                 "DefaultPanel\nDefaulting panels in your gh scene\n\nMathias Sønderskov 2020\nInspiration from Long Nguyen\nMinor details helped from Mahdiyar",
                 "Params",
                 "Util")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Default Text", "T", "Default Text", GH_ParamAccess.item, "");
            pManager.AddBooleanParameter("Reset", "R", "Reset", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            AddWire();
            //dynamic targetPanelComponent = OnPingDocument().FindComponent(targetPanelComponentGuid);
            //dynamic sourcePanelComponent = OnPingDocument().FindComponent(sourcePanelComponentGuid);
            string defaultText = "";
            bool run = false;

            DA.GetData(1, ref run);

            DA.GetData(0, ref defaultText);

            dynamic targetPanelComponent = OnPingDocument().FindObject(targetPanelComponentGuid, true);
            if (targetPanelComponent == null)
            {
                targetPanelComponentGuid = Guid.Empty;
            }

            //dynamic sourcePanelComponent = OnPingDocument().FindObject(sourcePanelComponentGuid, true);

            if (targetPanelComponent == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Please set a target panel component (in the right-click menu)");
                return;
            }
            /*if (sourcePanelComponent == null)
            {
                
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Please set a source panel component (in the right-click menu)");
                return;
            }*/

            if (run && targetPanelComponent != null)
                UpdatePanels(defaultText);
        }

        protected override Bitmap Icon => Properties.Resources.Icon_GhcDefaultPanel;

        public override GH_Exposure Exposure => GH_Exposure.primary;
        //public override void CreateAttributes() { Attributes = new GhcDefaultPanelAttributes(this); }


        public void UpdatePanels()
        {
            dynamic targetPanelComponent = OnPingDocument().FindObject(targetPanelComponentGuid, true);
            string defaultText = this.Params.Input[0].ToString();

            UpdatePanels(targetPanelComponent, defaultText);
        }

        public void UpdatePanels(string defaultText)
        {
            dynamic targetPanelComponent = OnPingDocument().FindObject(targetPanelComponentGuid, true);

            UpdatePanels(targetPanelComponent, defaultText);
        }
        public void UpdatePanels(dynamic targetPanelComponent, string defaultText)
        {
            if (targetPanelComponent != null)
            {
                targetPanelComponent.UserText = defaultText;
                targetPanelComponent.ExpireSolution(true);
                //ExpireSolution(true);
            }


        }

        public override bool Read(GH_IReader reader)
        {
            targetPanelComponentGuid = reader.GetGuid("targetPanelComponentGuid");

            return base.Read(reader);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetGuid("targetPanelComponentGuid", targetPanelComponentGuid);

            return base.Write(writer);
        }




        public override bool AppendMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendEnableItem(menu);
            Menu_AppendRuntimeMessages(menu);
            Menu_AppendSeparator(menu);



            Menu_AppendSeparator(menu);

            Menu_AppendItem(
                menu,
                targetPanelComponentGuid == Guid.Empty ? "      Set Target Panel -->" : "      Change Target Panel -->",
                delegate {
                    Instances.ActiveCanvas.ActiveInteraction = new GhcDefaultPanelCanvasInteration(Instances.ActiveCanvas, new GH_CanvasMouseEvent(), this);
                    //AddWire();
                    }, //true=target
                null,
                true,
                false);

            Menu_AppendItem(
                menu,
                "Disconnect Target",
                delegate {
                    targetPanelComponentGuid = Guid.Empty;
                    ExpireSolution(true);
                },
                null,
                 updateConnected(),
                false);

            /*Menu_AppendItem(
                menu,
                "Check Target",
                delegate { 
                    //Instances.ActiveCanvas.ActiveInteraction = new GhcDefaultPanelCanvasInterationPreview(Instances.ActiveCanvas, new GH_CanvasMouseEvent(), this);
                    AddWire();
                },
                null,
                updateConnected(),
                false);
                */

            /*if (OnPingDocument().FindObject(targetPanelComponentGuid, true) != null)
            {
                menu.Items[1].Visible = true;
                menu.Items[2].Visible = true;

            }

            else
            {
                menu.Items[1].Visible = false;
                menu.Items[2].Visible = false;
            }
            */

            return true;
        }

        public bool updateConnected()
        {
            if (OnPingDocument().FindObject(targetPanelComponentGuid, true) != null)
            {
                return true;
            }
            else
            {
                ExpireSolution(true);
                return false;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        /// 
        public override Guid ComponentGuid
        {
            get { return new Guid("02dd9895-add0-44ff-bd55-1cc33190d840"); }
        }

        public void AddWire()
        {
            Instances.ActiveCanvas.ActiveInteraction = new GhcDefaultPanelCanvasInterationPreview(Instances.ActiveCanvas, new GH_CanvasMouseEvent(), this);
        }
    }
}