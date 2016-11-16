using System.Linq;

namespace AnyStatus
{
    public class RootItem : Item
    {
        public void Initialize()
        {
            RestoreParentChildRelationship();

            if (Items != null)
                foreach (Folder folder in Items.Where(k => k is Folder))
                {
                    folder.CalculateState();
                }
        }
    }
}
