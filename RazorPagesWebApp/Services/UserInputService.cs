using RazorPagesWebApp.Models;

namespace RazorPagesWebApp.Services
{
    public static class UserInputService
    {
        public static CreateRoomInputModel PopulateCreateRoomInputModel(string inputList, int numberOfRepicks)
        {
            CreateRoomInputModel createRoomInputModel = new CreateRoomInputModel();

            createRoomInputModel.NumberOfRepicks = numberOfRepicks;

            string[] lines = inputList.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedLine))
                {
                    string name = trimmedLine.Substring(trimmedLine.IndexOf('.') + 1).Trim().ToLower();
                    if (name.EndsWith(" C", StringComparison.OrdinalIgnoreCase))
                    {
                        createRoomInputModel.Captains.Add(name.Substring(0, name.Length - 2).Trim());
                    }
                    else
                    {
                        createRoomInputModel.Players.Add(name);
                    }
                }
            }

            createRoomInputModel.Captains = createRoomInputModel.Captains.OrderBy(x => Guid.NewGuid()).ToList();
            createRoomInputModel.Players.Sort();

            return createRoomInputModel;
        }
    }
}
