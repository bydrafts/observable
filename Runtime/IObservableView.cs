#if DRAFTS_DATAVIEW
using UnityEngine;

namespace Drafts.DataView
{
    public class ObservableView : DataView<IReadOnlyObservable>
    {
        [SerializeField] private DataView dataView;

        protected override void Subscribe()
        {
            if (!dataView) return;
            dataView.SetData(Data.Value);
            Data.OnChanged += dataView.SetData;
        }

        protected override void Unsubscribe()
        {
            if (!dataView) return;
            dataView.SetData(null);
            Data.OnChanged -= dataView.SetData;
        }
    }
}
#endif