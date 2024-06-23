using InHues.Domain.DTO.Custom;
using Radzen.Blazor;
using System.Reflection;

namespace InHues.Components.ViewModels
{
    public class DataGrid<T> where T : class
    {
        public RadzenDataGrid<T> grid;
        public T? selected;
        public OdataResponse<T> DataList = new();

        public async Task EditRow(T item)
        {
            selected = item;
            await grid.EditRow(item);
        }

        public async Task DeleteRow(T item)
        {
            if (item == selected)
            {
                selected = null;
            }

            grid.CancelEditRow(item);
            await grid.Reload();
        }
        public async Task SaveRow(T item)
        {
            await grid.UpdateRow(item);
        }
        public void CancelEdit(T item)
        {
            if (item == selected)
            {
                selected = null;
            }

            grid.CancelEditRow(item);
        }
        public async Task InsertRow()
        {
            selected = Activator.CreateInstance<T>();
            await grid.InsertRow(selected);
        }
        public async Task Add(T data)
        {
            DataList.Values.Add(data);
            await grid.Reload();
        }
        public async Task Delete(T data)
        {
            PropertyInfo idProperty = typeof(T).GetProperty("Id");
            if (idProperty != null && idProperty.PropertyType == typeof(int))
            {
                int idValue = (int)idProperty.GetValue(data);
                DataList.Values.RemoveAll(x => (int)idProperty.GetValue(x) == idValue);
                await grid.Reload();
            }
        }
    }
}
