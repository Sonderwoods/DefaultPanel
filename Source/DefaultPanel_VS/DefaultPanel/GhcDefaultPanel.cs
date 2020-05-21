using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Script;
using Grasshopper.Kernel;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.CSharp.RuntimeBinder;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using System.Collections.Generic;

/*
 *  HIGHLY based on code by Long Nguyens ScriptSync (and he had some help from David Rutten), Thanks both of you.
 *  Also thanks to Mahdiyar - https://discourse.mcneel.com/u/Mahdiyar
 */



namespace DefaultPanel
{
    public partial class GhcDefaultPanel : GH_Component
    {
        GH_Document ghDocument = null;
        List<IGH_ActiveObject> ghActiveObjects;
        private Guid targetPanelComponentGuid = Guid.Empty;
        private Guid sourcePanelComponentGuid = Guid.Empty;

        public GhcDefaultPanel()
            : base(
                "DefaultPanel",
                "DefaultPanel",
                "Defaulting panels in your gh scene",
                "Params",
                "Util")
        {
        }


        protected override Bitmap Icon => Properties.Resources.Icon_GhcDefaultPanel;
        public override Guid ComponentGuid => new Guid("{e40ccee5-232e-44a0-9624-fb0ea34a833a}");
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override void CreateAttributes() { Attributes = new GhcDefaultPanelAttributes(this); }

        protected override void RegisterInputParams(GH_InputParamManager pManager) { }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager) { }



        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //dynamic targetPanelComponent = OnPingDocument().FindComponent(targetPanelComponentGuid);
            //dynamic sourcePanelComponent = OnPingDocument().FindComponent(sourcePanelComponentGuid);

            dynamic targetPanelComponent = OnPingDocument().FindObject(targetPanelComponentGuid, true);
            dynamic sourcePanelComponent = OnPingDocument().FindObject(sourcePanelComponentGuid, true);

            if (targetPanelComponent == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Please set a target panel component (in the right-click menu)");
                return;
            }
            if (sourcePanelComponent == null)
            {
                
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Please set a source panel component (in the right-click menu)");
                return;
            }

    
        }



        public override bool Write(GH_IWriter writer)
        {
            writer.SetGuid("targetPanelComponentGuid", targetPanelComponentGuid);
            writer.SetGuid("sourcePanelComponentGuid", sourcePanelComponentGuid);

            return base.Write(writer);
        }


        public override bool Read(GH_IReader reader)
        {
            targetPanelComponentGuid = reader.GetGuid("targetPanelComponentGuid");
            sourcePanelComponentGuid = reader.GetGuid("sourcePanelComponentGuid");

            return base.Read(reader);
        }

        public void UpdatePanels()
        {
            dynamic targetPanelComponent = OnPingDocument().FindObject(targetPanelComponentGuid, true);
            dynamic sourcePanelComponent = OnPingDocument().FindObject(sourcePanelComponentGuid, true);
            
            UpdatePanels(targetPanelComponent, sourcePanelComponent);
        }

        public void UpdatePanels(dynamic targetPanelComponent, dynamic sourcePanelComponent)
        {
            if (targetPanelComponent != null && sourcePanelComponent != null)
            {
                targetPanelComponent.UserText = sourcePanelComponent.UserText;
                targetPanelComponent.ExpireSolution(true);
                ExpireSolution(true);
            }
                

        }


        public void UpdateAllPanels()
        {
            ghDocument = OnPingDocument(); // also stolen from Long:)
            //ghActiveObjects = new List<IGH_ActiveObject>();
            List<IGH_ActiveObject> allActiveObjects = ghDocument.ActiveObjects();

            foreach (IGH_ActiveObject activeObject in allActiveObjects)
            {
                if (activeObject.ComponentGuid.ToString() == "e40ccee5-232e-44a0-9624-fb0ea34a833a") //being this type of object
                {
                    Guid target = activeObject.ComponentGuid;
                    //var myVar = activeObject.Reader["targetPanelComponentGuid"];

                    //MyFunction(myVar);
                }
            }

         }

        public override bool AppendMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(
                menu,
                "Reset content to defaults",
                delegate { UpdatePanels(); },
                null,
                OnPingDocument().FindObject(sourcePanelComponentGuid, true) != null && OnPingDocument().FindObject(targetPanelComponentGuid, true) != null,
                false);

            Menu_AppendItem(
                menu,
                "Default ALL panels in file(TODO)",
                delegate { },
                null,
                false,
                false);
            //Menu_AppendEnableItem(menu);
            //Menu_AppendSeparator(menu);
            //Menu_AppendRuntimeMessages(menu);
            Menu_AppendSeparator(menu);
            
            Menu_AppendSeparator(menu);
            
            Menu_AppendItem(
                menu,
                sourcePanelComponentGuid == Guid.Empty ? "<-- Set Source Panel      " : "<-- Change Source Panel      ",
                delegate { Instances.ActiveCanvas.ActiveInteraction = new GhcDefaultPanelCanvasInteration(Instances.ActiveCanvas, new GH_CanvasMouseEvent(), this, false); }, //false=source
                null,
                true,
                false);


            Menu_AppendItem(
                menu,
                targetPanelComponentGuid == Guid.Empty ? "      Set Target Panel -->" : "      Change Target Panel -->",
                delegate { Instances.ActiveCanvas.ActiveInteraction = new GhcDefaultPanelCanvasInteration(Instances.ActiveCanvas, new GH_CanvasMouseEvent(), this, true); }, //true=target
                null,
                true,
                false);

            Menu_AppendSeparator(menu);

            Menu_AppendItem(
                menu,
                "Disconnect Both",
                delegate { 
                    targetPanelComponentGuid = Guid.Empty;
                    sourcePanelComponentGuid = Guid.Empty;
                    ExpireSolution(true);
                },
                null,
                OnPingDocument().FindObject(targetPanelComponentGuid,true) != null || OnPingDocument().FindObject(sourcePanelComponentGuid, true) != null,
                false);

            menu.Items[0].Visible = false;
            if (OnPingDocument().FindObject(targetPanelComponentGuid, true) != null && OnPingDocument().FindObject(sourcePanelComponentGuid, true) != null)
                menu.Items[1].Visible = true;
            else
                menu.Items[1].Visible = false;

            if (OnPingDocument().FindObject(targetPanelComponentGuid, true) == null && OnPingDocument().FindObject(sourcePanelComponentGuid, true) == null)
                menu.Items[5].Visible = false;
            else
                menu.Items[5].Visible = true;

            return true;
        }
        


        
    }


}