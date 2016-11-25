using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AnyStatus.Tests
{
    [TestClass]
    public class ItemTests
    {
        [TestMethod]
        public void IsAncestorOf_Should_Return_True_When_Contains_Child()
        {
            var item = new Item();
            var child = new Item();

            item.Add(child);

            Assert.IsTrue(item.IsAncestorOf(child));
        }

        [TestMethod]
        public void IsAncestorOf_Should_Return_True_When_Contains_Descendant()
        {
            Item item = new Item(),
                 child = new Item(),
                 grandchild = new Item(),
                 greatGrandchild = new Item();

            item.Add(child);
            child.Add(grandchild);
            grandchild.Add(greatGrandchild);

            Assert.IsTrue(item.IsAncestorOf(greatGrandchild));
        }

        [TestMethod]
        public void MoveTo_Target_Should_Contain_Source()
        {
            var item = new Item();
            var folder = new Folder();
            var rootItem = new RootItem();

            rootItem.Add(item);
            rootItem.Add(folder);

            item.MoveTo(folder);

            Assert.IsTrue(folder.Contains(item));
        }

        [TestMethod]
        public void MoveTo_RootItem_Should_Contain_Source()
        {
            var item = new Item();
            var folder = new Folder();
            var rootItem = new RootItem();

            folder.Add(item);
            rootItem.Add(folder);

            item.MoveTo(rootItem);

            Assert.IsTrue(rootItem.Contains(item));
        }

        [TestMethod]
        public void Add_Should_Contain_Added_Item()
        {
            var item = new Item();
            var parent = new Item();

            parent.Add(item);

            Assert.IsTrue(parent.Contains(item));
        }

        [TestMethod]
        public void Add_Should_Set_Parent()
        {
            var item = new Item();
            var parent = new Item();

            parent.Add(item);

            Assert.AreSame(parent, item.Parent);
        }

        [TestMethod]
        public void Add_Should_Set_Id_If_Empty()
        {
            var item = new Item();
            var parent = new Item();

            parent.Add(item);

            Assert.AreNotEqual(Guid.Empty, item.Id);
        }

        [TestMethod]
        public void Add_Should_Not_Set_Id_If_Exists()
        {
            var id = Guid.NewGuid();
            var item = new Item() { Id = id };
            var parent = new Item();

            parent.Add(item);

            Assert.AreEqual(id, item.Id);
        }

        [TestMethod]
        public void Delete_Should_Remove_From_Parent_Collection()
        {
            var item = new Item();
            var parent = new Item();

            parent.Add(item);

            item.Delete();

            Assert.IsFalse(parent.Contains(item));
        }

        [TestMethod]
        public void MoveUp_Index_Should_Decrease()
        {
            var parent = new Item();

            var item1 = new Item();
            var item2 = new Item();

            parent.Add(item1);
            parent.Add(item2);

            Assert.IsTrue(parent.Items.IndexOf(item1) == 0);
            Assert.IsTrue(parent.Items.IndexOf(item2) == 1);

            item2.MoveUp();

            Assert.IsTrue(parent.Items.IndexOf(item1) == 1);
            Assert.IsTrue(parent.Items.IndexOf(item2) == 0);
        }

        [TestMethod]
        public void MoveDown_Index_Should_Increase()
        {
            var parent = new Item();

            var item1 = new Item();
            var item2 = new Item();

            parent.Add(item1);
            parent.Add(item2);

            Assert.IsTrue(parent.Items.IndexOf(item1) == 0);
            Assert.IsTrue(parent.Items.IndexOf(item2) == 1);

            item1.MoveDown();

            Assert.IsTrue(parent.Items.IndexOf(item1) == 1);
            Assert.IsTrue(parent.Items.IndexOf(item2) == 0);
        }

        [TestMethod]
        public void Clone_Should_Create_A_New_Object()
        {
            var item = new Item();

            var copy = item.Clone();

            Assert.AreNotSame(copy, item);
        }

        [TestMethod]
        public void ReplaceWith_Should_Replace_Source_With_Target()
        {
            var parent = new Item();
            var item1 = new Item();
            var item2 = new Item();

            parent.Add(item1);

            var index = parent.Items.IndexOf(item1);

            item1.ReplaceWith(item2);

            Assert.AreSame(parent.Items[index], item2);
        }

        [TestMethod]
        public void Collapse_Should_Set_IsExpanded_To_False()
        {
            var item = new Item() { IsExpanded = true };

            item.Collapse();

            Assert.IsFalse(item.IsExpanded);
        }

        [TestMethod]
        public void CollapseAll_Should_Set_Descendants_IsExpanded_To_False()
        {
            var item = new Item() { IsExpanded = true };
            var child = new Item() { IsExpanded = true };
            var grandchild = new Item() { IsExpanded = true };

            item.Add(child);
            child.Add(grandchild);

            item.CollapseAll();

            Assert.IsFalse(item.IsExpanded);
            Assert.IsFalse(child.IsExpanded);
            Assert.IsFalse(grandchild.IsExpanded);
        }

        [TestMethod]
        public void Expand_Should_Set_IsExpanded_To_True()
        {
            var item = new Item() { IsExpanded = false };

            item.Expand();

            Assert.IsTrue(item.IsExpanded);
        }

        [TestMethod]
        public void ExpandAll_Should_Set_Descendants_IsExpanded_To_True()
        {
            var item = new Item() { IsExpanded = false };
            var child = new Item() { IsExpanded = false };
            var grandchild = new Item() { IsExpanded = false };

            item.Add(child);
            child.Add(grandchild);

            item.ExpandAll();

            Assert.IsTrue(item.IsExpanded);
            Assert.IsTrue(child.IsExpanded);
            Assert.IsTrue(grandchild.IsExpanded);
        }

        [TestMethod]
        public void RestoreParentChildRelationship_Should_Restore_References()
        {
            var parent = new Item();
            var child = new Item();
            var grandchild = new Item();

            parent.Items = new ObservableCollection<Item>() { child };
            child.Items = new ObservableCollection<Item>() { grandchild };

            parent.RestoreParentChildRelationship();

            Assert.AreEqual(parent, child.Parent);
            Assert.AreEqual(child, grandchild.Parent);
        }

        [TestMethod]
        public void IsSchedulable_Should_Return_True_When_Item_Is_Valid_For_Scheduling()
        {
            var item = new Ping()
            {
                Name ="test",
                Host = "host",
                IsEnabled = true,
                Id = Guid.NewGuid(),
            };

            Assert.IsTrue(item.IsSchedulable());
        }

        [TestMethod]
        public void IsSchedulable_Should_Return_False_When_Item_Is_Disabled()
        {
            var item = new Ping()
            {
                IsEnabled = false,
                Id = Guid.NewGuid()
            };

            Assert.IsFalse(item.IsSchedulable());
        }

        [TestMethod]
        public void IsSchedulable_Should_Return_False_When_Id_Is_Empty()
        {
            var item = new Ping()
            {
                IsEnabled = true,
                Id = Guid.Empty
            };

            Assert.IsFalse(item.IsSchedulable());
        }

        [TestMethod]
        public void IsSchedulable_Should_Return_False_When_Item_Is__Not_IScheduledItem()
        {
            var item = new Item()
            {
                IsEnabled = true,
                Id = Guid.Empty
            };

            Assert.IsFalse(item.IsSchedulable());
        }

        [TestMethod]
        public void ContainsElements()
        {
            var item = new Item();
            var child = new Item();

            Assert.IsFalse(item.ContainsElements());

            item.Add(child);

            Assert.IsTrue(item.ContainsElements());
        }

        [TestMethod]
        public void ContainsElementsByType()
        {
            var folder = new Folder();
            var item1 = new Item();
            var item2 = new AppVeyorBuild();

            folder.Add(item1);

            Assert.IsFalse(folder.ContainsElements(typeof(AppVeyorBuild)));

            item1.Add(item2);

            Assert.IsTrue(folder.ContainsElements(typeof(AppVeyorBuild)));
        }

        [TestMethod]
        public void IsValid()
        {
            var item = new Item { Name = "Valid Item" };

            Assert.IsTrue(item.IsValid());

            item.Name = string.Empty;

            Assert.IsFalse(item.IsValid());
        }

        [TestMethod]
        public void Validate_Success()
        {
            var item = new Item() { Name = "Valid Item" };

            List<ValidationResult> results;

            Assert.IsTrue(item.Validate(out results));
            Assert.IsNotNull(results);
            Assert.IsFalse(results.Any());
        }

        [TestMethod]
        public void Validate_Invalid()
        {
            var item = new Item();

            List<ValidationResult> results;

            Assert.IsFalse(item.Validate(out results));
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void DefaultStateIsNone()
        {
            var item = new Item();

            Assert.AreEqual(State.None, item.State);
        }

        [TestMethod]
        public void WhenDisabled_StateIsDisabled()
        {
            var item = new Item { State = State.None };

            item.IsEnabled = false;

            Assert.AreEqual(State.Disabled, item.State);
        }

        [TestMethod]
        public void Duplicate_CreatesNewItem()
        {
            var folder = new Folder();
            var item = new Item();

            folder.Add(item);

            var clone = item.Duplicate();

            Assert.AreNotSame(item, clone);
        }

        [TestMethod]
        public void Duplicate_AddsNewItemToParent()
        {
            var folder = new Folder();
            var item = new Item();

            folder.Add(item);

            var clone = item.Duplicate();

            item.Parent.Items.Contains(clone);
        }

        [TestMethod]
        public void Duplicate_GeneratesNewName()
        {
            var folder = new Folder();
            var item = new Item { Name = "Name" };

            folder.Add(item);

            var clone1 = item.Duplicate();

            Assert.AreEqual("Name #1", clone1.Name);

            var clone2 = item.Duplicate();

            Assert.AreEqual("Name #2", clone2.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Duplicate_ItemMustHaveParent()
        {
            var item = new Item();

            item.Duplicate();
        }
    }
}