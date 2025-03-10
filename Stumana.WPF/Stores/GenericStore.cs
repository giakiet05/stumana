
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Stumana.WPF.Stores
{
    public class GenericStore<T> where T : class // Singleton store cho dữ liệu kiểu T
    {
        private static GenericStore<T> _instance;

        // Danh sách các đối tượng kiểu T
        public ObservableCollection<T> Items { get; set; }

        // Đối tượng được chọn
        private T _selectedItem;
        public T SelectedItem
        {
            get => _selectedItem;
            set => _selectedItem = value;
        }

        // Constructor private để đảm bảo Singleton
        private GenericStore()
        {
            Items = new ObservableCollection<T>();
        }

        // Instance Singleton
        public static GenericStore<T> Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GenericStore<T>();
                }
                return _instance;
            }
        }

        // Thêm một đối tượng mới vào danh sách
        public void Add(T newItem)
        {
            if (newItem == null) throw new ArgumentNullException(nameof(newItem));
            Items.Add(newItem);

        }

        // Cập nhật một đối tượng dựa trên điều kiện
        public void Update(T updatedItem, Func<T, bool> predicate)
        {
            if (updatedItem == null) throw new ArgumentNullException(nameof(updatedItem));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var existingItem = Items.FirstOrDefault(predicate);
            if (existingItem != null)
            {
                Items.Remove(existingItem);

                Items.Insert(0, updatedItem);
            }
        }


        // Xóa một đối tượng dựa trên điều kiện
        public void Delete(Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var existingItem = Items.FirstOrDefault(predicate);
            if (existingItem != null)
            {
                Items.Remove(existingItem);
            }

        }

        // Tải danh sách đối tượng kiểu T vào Store
        public void Load(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }

        public void Clear()
        {
            Items.Clear();
            SelectedItem = null;
        }
    }

}