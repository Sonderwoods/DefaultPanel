using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI.Canvas.Interaction;
using Grasshopper.Kernel;
using Point = System.Drawing.Point;

/*
 *  HIGHLY based on code by Long Nguyens ScriptSync (and he had some help from David Rutten), Thanks both of you.
 */


namespace DefaultPanel
{
    public partial class GhcDefaultPanel : GH_Component
    {

        protected class GhcDefaultPanelCanvasInteration : GH_AbstractInteraction
        {
            internal readonly GhcDefaultPanel ghcDefaultPanel;
            private PointF mousePosition = PointF.Empty;
            private readonly bool target = true;


            public GhcDefaultPanelCanvasInteration(GH_Canvas ghCanvas, GH_CanvasMouseEvent mouseEvent,
                GhcDefaultPanel ghcDefaultPanel, bool target)
                : base(ghCanvas, mouseEvent, true)
            {
                this.target = target;
                this.ghcDefaultPanel = ghcDefaultPanel;

                m_canvas.CanvasPostPaintObjects += canvasPostPaintObjects;
            }


            public override void Destroy()
            {
                m_canvas.CanvasPostPaintObjects -= canvasPostPaintObjects;
            }


            public override GH_ObjectResponse RespondToMouseMove(GH_Canvas ghCanvas, GH_CanvasMouseEvent e)
            {
                mousePosition = e.CanvasLocation;
                ghCanvas.Refresh();
                return GH_ObjectResponse.Handled;
            }


            public override GH_ObjectResponse RespondToMouseUp(GH_Canvas ghCanvas, GH_CanvasMouseEvent e)
            {
                mousePosition = e.CanvasLocation;
                IGH_DocumentObject targetDocumentObject = ghcDefaultPanel.OnPingDocument().FindObject(mousePosition, 1f);



                if (targetDocumentObject == null || targetDocumentObject == ghcDefaultPanel)
                    return GH_ObjectResponse.Release;
                
                if (targetDocumentObject.ComponentGuid.ToString() == "59e0b89a-e487-49f8-bab8-b5bab16be14c") //panel
                {
                    if (target)
                    {
                        ghcDefaultPanel.targetPanelComponentGuid = targetDocumentObject.InstanceGuid;
                        //MessageBox.Show("set target panel ok");
                    }
                    else
                    {
                        ghcDefaultPanel.sourcePanelComponentGuid = targetDocumentObject.InstanceGuid;
                        //MessageBox.Show("set source panel ok");
                        //MessageBox.Show(string.Format("{0}", ghcDefaultPanel.sourcePanelComponentGuid));
                    }

                    ghcDefaultPanel.ExpireSolution(true);
                    return GH_ObjectResponse.Release;
                }
                

                //ghcDefaultPanel.ExpireSolution(true);
                MessageBox.Show("Please select a panel");
                return GH_ObjectResponse.Release;
            }


            public override GH_ObjectResponse RespondToKeyDown(GH_Canvas sender, KeyEventArgs e)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                    case Keys.Cancel:
                        mousePosition = PointF.Empty;
                        return GH_ObjectResponse.Release;
                    default:
                        return GH_ObjectResponse.Ignore;
                }
            }


            private void canvasPostPaintObjects(GH_Canvas ghCanvas)
            {
                ghCanvas.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                
                if (!mousePosition.IsEmpty)
                    ghCanvas.Graphics.DrawLine(
                        new Pen(Color.Gray, 3f) { DashCap = DashCap.Round, DashPattern = new[] { 1f, 0.25f } },
                        ghcDefaultPanel.Attributes.Pivot,
                        mousePosition);
            }
        }
    }
}
