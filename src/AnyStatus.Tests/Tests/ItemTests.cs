using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus.Tests
{
    [TestClass]
    public class ItemTests
    {
        [TestMethod]
        public void IsParentOf()
        {
            var item = new Item();
            var child = new Item();

            item.Items = new ObservableCollection<Item>()
            {
                child
            };

            Assert.IsTrue(item.IsParentOf(child));
        }

        [TestMethod]
        public void MoveTo()
        {
            var item = new Item();
        }

        [TestMethod]
        public void Add()
        {
            var parent = new Item();
            var item = new Item();

            parent.Add(item);

            Assert.IsTrue(parent.Items.Contains(item));
        }

        [TestMethod]
        public void Delete()
        {
            var parent = new Item();
            var item = new Item();

            parent.Add(item);

            item.Delete();

            Assert.IsFalse(parent.Items.Contains(item));
        }

        [TestMethod]
        public void MoveUp()
        {
            var parent = new Item();

            var item1 = new Item();
            var item2 = new Item();

            parent.Add(item1);
            parent.Add(item2);

            item2.MoveUp();

            Assert.IsTrue(parent.Items.IndexOf(item2) == 0);
        }

        [TestMethod]
        public void MoveDown()
        {
            var parent = new Item();

            var item1 = new Item();
            var item2 = new Item();

            parent.Add(item1);
            parent.Add(item2);

            item1.MoveDown();

            Assert.IsTrue(parent.Items.IndexOf(item1) == 1);
        }

        [TestMethod]
        public void Clone()
        {
            var item = new Item();

            var copy = item.Clone();

            Assert.AreNotSame(copy, item);
        }

        [TestMethod]
        public void ReplaceWith()
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
        public void Collapse()
        {
            var item = new Item() { IsExpanded = true };

            item.Collapse();

            Assert.IsFalse(item.IsExpanded);
        }

        [TestMethod]
        public void CollapseAll()
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
        public void Expand()
        {
            var item = new Item() { IsExpanded = false };

            item.Expand();

            Assert.IsTrue(item.IsExpanded);
        }

        [TestMethod]
        public void ExpandAll()
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
        public void RestoreParentChildRelationship()
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
        public void IsSchedulable()
        {
            var item = new Ping()
            {
                IsEnabled = true,
                Id = Guid.NewGuid()
            };

            Assert.IsTrue(item.IsSchedulable());
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
        public void WhenEnabled_StateIsNone()
        {
            var item = new Item { State = State.Unknown };

            item.IsEnabled = true;

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