﻿using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using TaskManager.Shared;

namespace TaskManager.Client.Pages
{
    public partial class Index
    {
        [Inject] public HttpClient Http { get; set; }
        private IList<TaskItem>? tasks;
        private string? error;
        private string? newTask;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                string requestUri = "api/TaskItems";
                tasks =
                     await Http.GetFromJsonAsync<IList<TaskItem>>
                     (requestUri);
            }
            catch (Exception)
            {
                error = "Error Encountered";
            };
        }

        private async Task AddTask()
        {
            if (!string.IsNullOrWhiteSpace(newTask))
            {
                TaskItem newTaskItem = new TaskItem
                {
                    TaskName = newTask,
                    IsComplete = false
                };
                tasks!.Add(newTaskItem);
                string requestUri = "api/TaskItems";
                var response = await Http.PostAsJsonAsync(requestUri, newTaskItem);
                if (response.IsSuccessStatusCode)
                {
                    newTask = string.Empty;
                }
                else
                {
                    error = response.ReasonPhrase;
                };
            };
        }

        private async Task DeleteTask(TaskItem taskItem)
        {
            tasks!.Remove(taskItem);
            StateHasChanged();
            string requestUri =
            $"api/TaskItems/{taskItem.TaskItemId}";
            var response = await Http.DeleteAsync(requestUri);
            if (!response.IsSuccessStatusCode)
            {
                error = response.ReasonPhrase;
            };
        }

        private async Task CheckboxChecked(TaskItem task)
        {
            task.IsComplete = !task.IsComplete;
            string requestUri = $"api/TaskItems/{task.TaskItemId}";
            var response =
            await Http.PutAsJsonAsync<TaskItem>
            (requestUri, task);
            if (!response.IsSuccessStatusCode)
            {
                error = response.ReasonPhrase;
            };
        }

    }
}
