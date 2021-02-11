using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tasklify.Contracts;

namespace takehome.tests
{
    public class TestDataHelper
    {
        public static async Task<IList<TasklifyTask>> GetTestGetAllTask()
        {
            string data = "[{\"id\":1,\"summary\":\"SUMMARY SUMMARY SUMMARY\",\"description\":\"Description Description Description.\",\"assignee_id\":1},{\"id\":2,\"summary\":\"I AM A SUMMARY\",\"description\":\"jk lol rofl 1337.\",\"assignee_id\":0}]";
            return JsonConvert.DeserializeObject<IList<TasklifyTask>>(data);
        }

        public static async Task<TasklifyTask> GetTestGetById()
        {
            string data = "{\"id\":1,\"summary\":\"I AM A SUMMARY\",\"description\":\"jk lol rofl 1337.\",\"assignee_id\":0}";
            return JsonConvert.DeserializeObject<TasklifyTask>(data);
        }

        public static TasklifyTask GetTestTaskForEdit()
        {
            string data = "{\"id\":1,\"summary\":\"SUMMARY\",\"description\":\"Description.\",\"assignee_id\":0}";
            return JsonConvert.DeserializeObject<TasklifyTask>(data);
        }

        public static async Task<TasklifyTask> GetTestTaskForEditAfterUpdate()
        {
            string data = "{\"id\":1,\"summary\":\"SUMMARY Updated\",\"description\":\"Description Updated\",\"assignee_id\":1}";
            return JsonConvert.DeserializeObject<TasklifyTask>(data);
        }

        public static TasklifyTask GetTestTaskForAdd()
        {
            string data = "{\"id\":0,\"summary\":\"SUMMARY Add\",\"description\":\"Description Add\",\"assignee_id\":1}";
            return JsonConvert.DeserializeObject<TasklifyTask>(data);
        }

        public static TasklifyTask GetTestTaskForAddNoSummary()
        {
            string data = "{\"id\":0,\"summary\":\"\",\"description\":\"Description Add\",\"assignee_id\":1}";
            return JsonConvert.DeserializeObject<TasklifyTask>(data);
        }

        public static TasklifyTask GetTestTaskForAddLongSummaryAndDescription()
        {
            string data = "{\"id\":0,\"summary\":\"SUMMARY Add SUMMARY Add SUMMARY Add SUMMARY AddSUMMARY AddSUMMARY Add SUMMARY AddSUMMARY Add SUMMARY Add SUMMARY Add SUMMARY Add SUMMARY Add\",\"description\":\"Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add\",\"assignee_id\":1}";
            return JsonConvert.DeserializeObject<TasklifyTask>(data);
        }

        public static TasklifyTask GetTestTaskForAddInvalidAssignee()
        {
            string data = "{\"id\":0,\"summary\":\"SUMMARY Add \",\"description\":\"Description Add \",\"assignee_id\":100}";
            return JsonConvert.DeserializeObject<TasklifyTask>(data);
        }

        public static async Task<TasklifyTask> GetTestTaskForAddSuccess()
        {
            string data = "{\"id\":1,\"summary\":\"SUMMARY Add\",\"description\":\"Description Add\",\"assignee_id\":1}";
            return JsonConvert.DeserializeObject<TasklifyTask>(data);
        }

        public static async Task<IList<TasklifyUser>> GetTestGetAllUser()
        {
            string data = "[{\"id\":1,\"email\":\"test@bridgitsolutions.com\",\"name\":\"Testy McTester\"},{\"id\":2,\"email\":\"test-2@bridgitsolutions.com\",\"name\":\"Jill McTester\"}]";
            return JsonConvert.DeserializeObject<IList<TasklifyUser>>(data);
        }

        public static string PutExceptionMessage()
        {
            return "Error while updating task with id:5";
        }

    }
}
