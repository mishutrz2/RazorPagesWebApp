using RazorPagesWebApp.Models;

namespace RazorPagesWebApp.Services
{
    public static class UserInputService
    {
        public static CreateRoomInputModel PopulateCreateRoomInputModel(string inputList, string PickOrder)
        {
            CreateRoomInputModel createRoomInputModel = new CreateRoomInputModel();

            createRoomInputModel.PickOrder = PickOrder;

            string[] lines = inputList.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedLine))
                {
                    string name = trimmedLine.Substring(trimmedLine.IndexOf('.') + 1).Trim().ToLower();
                    if (name.EndsWith("C", StringComparison.OrdinalIgnoreCase))
                    {
                        createRoomInputModel.Captains.RemoveAll(x => x == "");
                        createRoomInputModel.Captains.Add(name.Substring(0, name.Length - 2).Trim());
                        var rng = new Random();
                        createRoomInputModel.Captains = createRoomInputModel.Captains
                            .OrderBy(x => rng.Next())
                            .ToList();
                    }
                    else if (name.EndsWith("C1", StringComparison.OrdinalIgnoreCase) ||
                        name.EndsWith("C2", StringComparison.OrdinalIgnoreCase) ||
                        name.EndsWith("C3", StringComparison.OrdinalIgnoreCase))
                    {
                        char lastChar = name[name.Length - 1];
                        int index = lastChar - '0'; // Converts '2' → 2
                        createRoomInputModel.Captains[index-1] = name.Substring(0, name.Length - 2).Trim();
                    }
                    else
                    {
                        createRoomInputModel.Players.Add(name);
                    }
                }
            }

            createRoomInputModel.Players.Sort();

            return createRoomInputModel;
        }
    }
}
