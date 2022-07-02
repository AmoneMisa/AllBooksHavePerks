using System;
using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Noggog;

namespace AllBooksHavePerks
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "All Books Have Perks.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            var skills = Enum.GetValues<Skill>();
            var random = new Random();

            foreach (var bookContext in state.LoadOrder.PriorityOrder.Book().WinningContextOverrides())
            {
                if (bookContext.Record.Teaches is BookTeachesNothing)
                {
                    Console.WriteLine($"Processing {bookContext.Record.Name}");

                    var bookMutable = bookContext.GetOrAddAsOverride(state.PatchMod);
                    var bookSkill = new BookSkill();
                    var index = (int)(random.NextDouble() * skills.Length);
                    bookSkill.Skill = skills[index];
                    bookMutable.Teaches = bookSkill;
                    
                    Console.WriteLine($"New value: {bookSkill.Skill}");
                }
            }
        }
    }
}