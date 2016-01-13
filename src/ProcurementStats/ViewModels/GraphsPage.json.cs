using Starcounter;
using Simplified.Ring6;

namespace ProcurementStats {
    partial class GraphsPage : Page {
        public void RefreshData() {
            Graphs = Db.SQL("SELECT i FROM Simplified.Ring6.ProcurementGraph i");
        }

        // This attribute indicates which part of JSON tree 
        // is represented by this class
        [GraphsPage_json.Graphs]
        partial class GraphsItemPage {
            protected override void OnData() {
                ProcurementGraph data = (ProcurementGraph)Data;
                base.OnData();
                this.Url = string.Format("/ProcurementStats/Details/{0}", this.Key);
                this.DateFromString = data.DateFrom.ToShortDateString();
                this.DateToString = data.DateTo.ToShortDateString();
            }
        }
    }
}
