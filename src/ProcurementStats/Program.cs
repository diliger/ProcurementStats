using System;
using Starcounter;

namespace ProcurementStats {
    class Program {
        static void Main() {
            Handle.GET("/ProcurementStats", () => {
                MasterPage master;

                if (Session.Current != null && Session.Current.Data != null) {
                    master = (MasterPage)Session.Current.Data;
                } else {
                    master = new MasterPage();

                    if (Session.Current != null) {
                        master.Html = "/ProcurementStats/viewmodels/LauncherWrapperPage.html";
                        master.Session = Session.Current;
                    } else {
                        master.Html = "/ProcurementStats/viewmodels/MasterPage.html";
                        master.Session = new Session(SessionOptions.PatchVersioning);
                    }

                    master.RecentGraphs = new GraphsPage() {
                        Html = "/ProcurementStats/viewmodels/GraphsPage.html"
                    };
                }

                ((GraphsPage)master.RecentGraphs).RefreshData();
                master.FocusedGraph = null;

                return master;
            });

            Handle.GET("/ProcurementStats/NewGraph", () => {
                MasterPage master = Self.GET<MasterPage>("/ProcurementStats");
                master.FocusedGraph = Db.Scope<GraphPage>(() => {
                    GraphPage page = new GraphPage() {
                        Html = "/ProcurementStats/viewmodels/GraphPage.html",
                        Data = new Simplified.Ring6.ProcurementGraph() { DateTo = DateTime.Now.Date, DateFrom = DateTime.Now.AddMonths(-1).Date }
                    };

                    page.Saved += (s, a) => {
                        ((GraphsPage)master.RecentGraphs).RefreshData();
                    };

                    page.Deleted += (s, a) => {
                        ((GraphsPage)master.RecentGraphs).RefreshData();
                    };

                    return page;
                });
                return master;
            });

            Handle.GET("/ProcurementStats/Details/{?}", (string Key) => {
                MasterPage master = Self.GET<MasterPage>("/ProcurementStats");
                master.FocusedGraph = Db.Scope<GraphPage>(() => {
                    var page = new GraphPage() {
                        Html = "/ProcurementStats/viewmodels/GraphPage.html",
                        Data = Db.SQL<Simplified.Ring6.ProcurementGraph>(@"SELECT i FROM Simplified.Ring6.ProcurementGraph i WHERE Key = ?", Key).First
                    };

                    page.Saved += (s, a) => {
                        ((GraphsPage)master.RecentGraphs).RefreshData();
                    };

                    page.Deleted += (s, a) => {
                        ((GraphsPage)master.RecentGraphs).RefreshData();
                    };
                    page.RequestGraph();

                    return page;
                });
                return master;
            });

            //Handle.GET("/ProcurementStats/Only/{?}", (string Key) => {
            //    GraphPage page = new GraphPage() {
            //        Html = "/ProcurementStats/viewmodels/GraphPage.html",
            //        Data = Db.SQL<Simplified.Ring6.ProcurementGraph>(@"SELECT i FROM Simplified.Ring6.ProcurementGraph i WHERE i.Key = ?", Key).First
            //    };

            //    return page;
            //});

            Handle.GET("/ProcurementStats/menu", () => {
                return new Page() { Html = "/ProcurementStats/viewmodels/AppMenuPage.html" };
            });

            Handle.GET("/ProcurementStats/app-name", () => {
                return new AppName();
            });

            UriMapping.Map("/ProcurementStats/app-name", UriMapping.MappingUriPrefix + "/app-name");
            UriMapping.Map("/ProcurementStats/menu", UriMapping.MappingUriPrefix + "/menu");
        }
    }
}