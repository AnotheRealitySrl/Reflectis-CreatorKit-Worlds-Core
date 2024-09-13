using Reflectis.SDK.CreatorKit;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKitEditor
{
    [Widget(typeof(SyncedVariablesEventNodes))]
    public class SyncedVariablesEventNodesWidget : UnitWidget<SyncedVariablesEventNodes>
    {
        public SyncedVariablesEventNodesWidget(FlowCanvas canvas, SyncedVariablesEventNodes unit) : base(canvas, unit)
        {
            nameInspectorConstructor = (metadata) => new VariableNameInspector(metadata, GetNameSuggestions);
        }

        protected override NodeColorMix baseColor => new NodeColorMix(NodeColor.Green);

        private VariableNameInspector nameInspector;
        private Func<Metadata, VariableNameInspector> nameInspectorConstructor;

        public override Inspector GetPortInspector(IUnitPort port, Metadata metadata)
        {
            if (port == unit.VariableName)
            {
                // This feels so hacky. The real holy grail here would be to support attribute decorators like Unity does.
                InspectorProvider.instance.Renew(ref nameInspector, metadata, nameInspectorConstructor);

                return nameInspector;
            }

            return base.GetPortInspector(port, metadata);
        }

        private IEnumerable<string> GetNameSuggestions()
        {
            return EditorVariablesUtility.GetVariableNameSuggestions(VariableKind.Object, reference);
        }

    }

    [Widget(typeof(OnSyncedVariableInit))]
    public class OnSyncedVariableInitWidget : UnitWidget<OnSyncedVariableInit>
    {
        public OnSyncedVariableInitWidget(FlowCanvas canvas, OnSyncedVariableInit unit) : base(canvas, unit)
        {
            nameInspectorConstructor = (metadata) => new VariableNameInspector(metadata, GetNameSuggestions);
        }
        protected override NodeColorMix baseColor => new NodeColorMix(NodeColor.Green);
        private VariableNameInspector nameInspector;
        private Func<Metadata, VariableNameInspector> nameInspectorConstructor;

        public override Inspector GetPortInspector(IUnitPort port, Metadata metadata)
        {
            if (port == unit.VariableName)
            {
                // This feels so hacky. The real holy grail here would be to support attribute decorators like Unity does.
                InspectorProvider.instance.Renew(ref nameInspector, metadata, nameInspectorConstructor);

                return nameInspector;
            }

            return base.GetPortInspector(port, metadata);
        }

        private IEnumerable<string> GetNameSuggestions()
        {
            return EditorVariablesUtility.GetVariableNameSuggestions(VariableKind.Object, reference);
        }

    }

    [Widget(typeof(OnSyncedVariableInitEventUnit))]
    public class OnSyncedVariableInitEventUnitWidget : UnitWidget<OnSyncedVariableInitEventUnit>
    {
        public OnSyncedVariableInitEventUnitWidget(FlowCanvas canvas, OnSyncedVariableInitEventUnit unit) : base(canvas, unit)
        {
            nameInspectorConstructor = (metadata) => new VariableNameInspector(metadata, GetNameSuggestions);
        }
        protected override NodeColorMix baseColor => new NodeColorMix(NodeColor.Green);
        private VariableNameInspector nameInspector;
        private Func<Metadata, VariableNameInspector> nameInspectorConstructor;

        public override Inspector GetPortInspector(IUnitPort port, Metadata metadata)
        {
            if (port == unit.VariableName)
            {
                // This feels so hacky. The real holy grail here would be to support attribute decorators like Unity does.
                InspectorProvider.instance.Renew(ref nameInspector, metadata, nameInspectorConstructor);

                return nameInspector;
            }

            return base.GetPortInspector(port, metadata);
        }

        private IEnumerable<string> GetNameSuggestions()
        {
            return EditorVariablesUtility.GetVariableNameSuggestions(VariableKind.Object, reference);
        }

    }
}
