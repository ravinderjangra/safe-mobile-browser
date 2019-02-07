using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SafeMobileBrowser.Models
{
    public class TabPageStore
    {
        public ObservableCollection<TabPage> TabPages { get; set; }

        public TabPageStore()
        {
            TabPages = new ObservableCollection<TabPage>();
        }

        public void AddPage(TabPage page)
        {
            TabPages.Add(page);
        }

        public void AddPageScreenShot(string pageTitle, byte[] screenshot)
        {
            var tabPage = TabPages.Where(p => p.PageTitle == pageTitle).First();
            if (tabPage != null)
            {
                tabPage.PageScreenShot = screenshot;
            }
        }
    }
}
