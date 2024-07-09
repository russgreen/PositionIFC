using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;

namespace IFCLocation.Commands;
[Transaction(TransactionMode.Manual)]
public class CommandPositionIFC : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        App.CachedUiApp = commandData.Application;
        App.RevitDocument = commandData.Application.ActiveUIDocument.Document;

        //select a linked IFC file
        var linkInstance = App.RevitDocument.GetElement(
            commandData.Application.ActiveUIDocument.Selection.PickObject(
                Autodesk.Revit.UI.Selection.ObjectType.Element,
                new IFCPickFilter(),
                "Please select a linked IFC model"));

        if (linkInstance == null)
        {
            return Result.Failed;
        }

        var linkedModel = linkInstance as RevitLinkInstance;
        var linkedTransform = linkedModel.GetTotalTransform();
        var linkedDocument = linkedModel.GetLinkDocument();

        //get the base point of the linked model
        var linkedCollector = new FilteredElementCollector(linkedDocument);
        var linkedBasePoint = linkedCollector.OfClass(typeof(BasePoint))
                                             .Cast<BasePoint>()
                                             .FirstOrDefault(bp => !bp.IsShared);

        if (linkedBasePoint == null)
        {
            return Result.Failed;
        }

        var hostTransform = App.RevitDocument.ActiveProjectLocation.GetTotalTransform();

        // Get the base point of the host model
        var hostCollector = new FilteredElementCollector(App.RevitDocument);
        var hostBasePoint = hostCollector.OfClass(typeof(BasePoint))
                                         .Cast<BasePoint>()
                                         .FirstOrDefault(bp => !bp.IsShared);

        if (hostBasePoint == null)
        {
            return Result.Failed;
        }

        // Calculate the translation vector
        var linkedBasePointCoordinatesInHost = linkedTransform.Origin;
        var linkInsertionPoint = hostTransform.OfPoint(linkedBasePoint.SharedPosition);
        var translationVector = linkInsertionPoint - linkedBasePointCoordinatesInHost;

        // Apply the translation to the linked model
        using (Transaction trans = new Transaction(App.RevitDocument, "Move Linked Model"))
        {
            trans.Start();
            ElementTransformUtils.MoveElement(App.RevitDocument, linkedModel.Id, translationVector);
            trans.Commit();
        }

        return Result.Succeeded;
    }


}
