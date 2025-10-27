using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snakegame
{
    internal class MainClass
    {
        static async Task Main(string[] args)
        {
            var llm = new LLMService();
            string welcomePrompt = "你是一个贪吃蛇游戏的AI助手，请用中文生成一句欢迎语和一条游戏技巧，不超过50字。";
            string welcome = await llm.GetResponseAsync(welcomePrompt);
            Console.WriteLine(welcome);
            Console.WriteLine("按任意键开始游戏...");
            Console.ReadKey();

            var game = new SnakeGame();
            game.Run();

            // 游戏结束后调用LLM总结
            string summaryPrompt = $"玩家得分{game.Score}，请用鼓励的语气给出一句个性化游戏总结，不超过40字。";
            string summary = await llm.GetResponseAsync(summaryPrompt);
            Console.WriteLine(summary);
        }
    }
}
