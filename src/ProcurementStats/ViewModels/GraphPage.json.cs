using System;
using Starcounter;
using Simplified.Ring6;

namespace ProcurementStats {
    partial class GraphPage : Page, IBound<Simplified.Ring6.ProcurementGraph> {
        public event EventHandler Saved;
        public event EventHandler Deleted;

        [GraphPage_json.GraphValues]
        public partial class GraphPageValues : Json, IBound<GraphValue> {
        }

        void Handle(Input.Save action) {
            BuildGraph();
            Transaction.Commit();
            OnSaved();
            //redirect to the new URL
            RedirectUrl = "/ProcurementStats/Details/" + Data.Key;
        }

        void Handle(Input.Cancel action) {
            Transaction.Rollback();
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
            RedirectUrl = "/ProcurementStats"; //redirect to the home URL        
        }

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
            
        }
    }
}
