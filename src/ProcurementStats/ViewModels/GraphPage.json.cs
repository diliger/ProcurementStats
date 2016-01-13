using System;
using System.Linq;
using Starcounter;
using Simplified.Ring6;
using System.Collections.Generic;

namespace ProcurementStats {
    partial class GraphPage : Page, IBound<Simplified.Ring6.ProcurementGraph> {
        public event EventHandler Saved;
        public event EventHandler Deleted;

        [GraphPage_json.GraphValues]
        public partial class GraphPageValues : Json, IBound<GraphValue> {
        }

        public void RequestGraph() {
            this.Graph = Self.GET<Json>(string.Format("{0}/{1}/{2}", UriMapping.OntologyMappingUriPrefix, typeof(Simplified.Ring6.Graph).FullName, Data.Key), () => new Page());
        }

        void Handle(Input.Save action) {
            BuildGraph();
            Transaction.Commit();
            OnSaved();
            //redirect to the new URL
            RedirectUrl = "/ProcurementStats/Details/" + Data.Key;

            RequestGraph();
        }

        void Handle(Input.Cancel action) {
            //Transaction.Rollback();
            RedirectUrl = "/ProcurementStats";
        }

        void Handle(Input.Delete action) {
            if (Data == null)
                return;

            foreach (var row in Data.GraphValues) {
                row.Delete();
            }
            Data.Delete();
            Transaction.Commit();
            OnDeleted();

            this.GraphValues.Clear();
            this.Data = null;
            RedirectUrl = "/ProcurementStats"; //redirect to the home URL        
        }

        //void Handle(Input.Build action) {
        //    RequestGraph();
        //}

        protected void OnSaved() {
            if (this.Saved != null) {
                this.Saved(this, EventArgs.Empty);
            }
        }

        protected void OnDeleted() {
            if (this.Deleted != null) {
                this.Deleted(this, EventArgs.Empty);
            }
        }

        private void BuildGraph() {
            //Simplified.Ring4.PurchaseOrderItem
            DateTime dateTo = Data.DateTo.AddDays(1);
            DateTime dateFrom = Data.DateFrom;
            Dictionary<string, decimal> dataItems =
                Db.SQL<Simplified.Ring4.PurchaseOrderItem>("SELECT i FROM Simplified.Ring4.PurchaseOrderItem i WHERE i.PurchaseOrder.BeginTime >= ? AND i.PurchaseOrder.BeginTime < ?", dateFrom, dateTo)
                .GroupBy(val => val.Name.ToLower()).ToDictionary(val => val.Key, val => val.Sum(x => x.Quantity));

            int i = 0;
            List<GraphValue> values = Data.GraphValues.ToList();
            foreach (KeyValuePair<string, decimal> item in dataItems.OrderBy(val => val.Key)) {
                GraphValue value = null;
                if (i < values.Count) {
                    value = values[i];
                } else {
                    value = new GraphValue() { Graph = Data };
                }
                value.XValue = i;
                value.XLabel = item.Key;
                value.YValue = item.Value;

                i++;
            }

            for (; i < values.Count; i++) {
                values[i].Delete();
            }
        }
    }
}
