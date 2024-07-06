using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelliCollabUI.Models
{
    public class BotRespFromApi
    {
        public string label { get; set; }
        public decimal score { get; set; }
    }

    public class DefaultAnswer
    {
        public static string WellcomingAnswer()
        {
            List<string> staringMsg = new List<string> {
                "🤖 Greetings! I'm your AI companion here to provide assistance. Feel free to ask me anything you need help with.",
                "🤖 Hello there! I'm an AI chatbot designed to answer your questions and provide support. What can I assist you with today?",
                "🤖 Welcome to our chat! I'm an AI bot at your service. Let's tackle your queries together!",
                "🤖 Hi! I'm your AI chat companion. Don't hesitate to ask me anything, from simple questions to complex inquiries.",
                "🤖 Hey! I'm here to lend a virtual hand. Ask me anything you'd like, and I'll do my best to provide you with the answers you need.",
                "🤖 Greetings, human! I'm an AI designed to assist you. How can I make your day easier?",
                "🤖 Welcome aboard! As your AI chat assistant, I'm here to help. Feel free to reach out to me anytime you need assistance.",
                "🤖 Hello! I'm your AI companion, equipped with knowledge and ready to assist you. Ask away!",
                "🤖 Hey there! I'm your virtual assistant. Ask me anything, and I'll provide you with accurate and timely responses.",
                "🤖 Greetings, traveler! I'm an AI guide here to assist you on your journey. Let's embark on an adventure of knowledge together!"
            };

            Random random = new Random();
            int randomIndex = random.Next(0, staringMsg.Count);
            string selectedResponse = staringMsg[randomIndex];

            return selectedResponse;
        }

        public static string NotTrainAnswer()
        {
            List<string> notTrainAns = new List<string> {
                "🤖 I'm sorry, but I haven't been trained on how to handle that specific query. Is there something else I can assist you with?",
                "🤖 I'm afraid I don't have a prepared response for that question. Can I help you with something else?",
                "🤖 It seems I haven't been taught how to answer that question yet. Is there anything else you'd like to know?",
                "🤖 Hmm, it appears this is beyond my current training. Feel free to ask about something else!",
                "🤖 I'm sorry, I don't have information on that topic at the moment. What else can I assist you with?",
                "🤖 Oops! It looks like I'm not trained for this query. Is there another question you'd like to ask?",
                "🤖 Ah, I haven't been briefed on that. Could you ask me something else instead?",
                "🤖 Unfortunately, I haven't learned about that yet. Is there a different topic you'd like to explore?",
                "🤖 I'm still learning, and it seems I haven't covered that topic. Can I help with anything else?"
            };

            Random random = new Random();
            int randomIndex = random.Next(0, notTrainAns.Count);
            string selectedResponse = notTrainAns[randomIndex];

            return selectedResponse;
        }

        public static string ErrorAnswer()
        {
            List<string> notTrainAns = new List<string> {
                 "ANS: Oops! It seems there's a technical glitch on our end. We're working to resolve it. Please try again later."
                ,"ANS: Sorry, we're experiencing some technical difficulties right now. We're aware of the issue and are working on fixing it."
                ,"ANS: Error: We're encountering some unexpected errors. Our team is investigating. We appreciate your patience."
                ,"ANS: Uh-oh! It appears there's a hiccup in the system. We're looking into it and will have it sorted out soon."
                ,"ANS: We're sorry, but it seems like something's gone wrong. Our team is actively working on resolving the issue."
                ,"ANS: Error: It seems we've hit a snag. Rest assured, we're on it and will have things back to normal shortly."
                ,"ANS: Oops! Looks like there's a technical gremlin causing trouble. We're rolling up our sleeves to fix it ASAP."
                ,"ANS: Sorry for the inconvenience! We're experiencing some technical issues, but we're working diligently to resolve them."
                ,"ANS: Error: Our apologies, but it seems we're encountering some technical difficulties. We appreciate your patience as we work to fix them."
                ,"ANS: We're currently experiencing some technical challenges. Hang tight while we work on getting things back up and running smoothly."
            };

            Random random = new Random();
            int randomIndex = random.Next(0, notTrainAns.Count);
            string selectedResponse = notTrainAns[randomIndex];

            return selectedResponse;
        }

        public static string EndingMsg()
        {
            List<string> notTrainAns = new List<string> {
                 "If you have any more questions or need further assistance, don't hesitate to ask!"
                ,"I'm always here to lend a helping hand whenever you need it."
                ,"Don't hesitate to reach out if there's anything else I can assist you with!"
                ,"Wishing you a fantastic day ahead! Remember, I'm just a message away if you need anything else."
                ,"Thanks for chatting! If you ever have more questions, I'm here to provide answers."
                ,"It's been great assisting you! Let me know if there's anything else I can do for you."
                ,"Until next time, take care and feel free to come back if you have more queries or tasks!"
                ,"Thank you for the conversation! If you ever need assistance again, I'll be here."
                ,"Signing off for now! Remember, you can always return if you need assistance in the future."
                ,"I'm here to help whenever you need support or guidance. Have a wonderful day!"
            };

            Random random = new Random();
            int randomIndex = random.Next(0, notTrainAns.Count);
            string selectedResponse = notTrainAns[randomIndex];

            return selectedResponse;
        }

        public static string ThanksMsg()
        {
            List<string> notTrainAns = new List<string> {
                 "You're welcome!"
                ,"Anytime!"
                ,"No problem!"
                ,"Happy to help!"
                ,"Glad I could assist!"
                ,"Not a problem at all!"
                ,"It was my pleasure!"
                ,"Don't mention it!"
                ,"You're very welcome!"
                ,"Always here for you!"
            };

            Random random = new Random();
            int randomIndex = random.Next(0, notTrainAns.Count);
            string selectedResponse = notTrainAns[randomIndex] +" "+ EndingMsg();

            return selectedResponse;
        }

        public static string NegativeMsg()
        {
            List<string> notTrainAns = new List<string> {
                 "I'm sorry, could you please provide more context?"
                ,"I'm a bit confused, could you clarify?"
                ,"I'm not sure I follow, could you explain further?"
                ,"I'm having trouble understanding, could you elaborate?"
                ,"I'm not quite getting it, could you give more details?"
                ,"Could you help me understand the situation better?"
                ,"I'm feeling a bit lost, could you give me more information?"
                ,"I'm having difficulty grasping the context, could you break it down?"
                ,"I'm having trouble connecting the dots, could you shed some light?"
                ,"I'm struggling to understand, could you provide some context clues?"
            };

            Random random = new Random();
            int randomIndex = random.Next(0, notTrainAns.Count);
            string selectedResponse = notTrainAns[randomIndex];

            return selectedResponse;
        }

        public static string HeyMsg()
        {
            List<string> notTrainAns = new List<string> {
                 "Hello! {name} How can I assist you today?"
                ,"Hey there! {name} What's on your mind?"
                ,"Howdy! {name} What brings you here?"
                ,"Salutations! {name} How may I help you?"
                ,"Hiya! {name} How can I be of service?"
                ,"Yo! {name} What's up?"
                ,"Wassup! {name} How can I assist you?"
                ,"Aloha! {name} How can I help you today?"
                ,"Greetings! {name} What can I do for you?"
                ,"Hey! {name} How can I assist you today?"
            };

            Random random = new Random();
            int randomIndex = random.Next(0, notTrainAns.Count);
            string selectedResponse = notTrainAns[randomIndex];

            return selectedResponse;
        }

        public static string NotMatchingAnswer()
        {
            List<string> notTrainAns = new List<string> {
                "I'm sorry, but I'm having trouble understanding your question.Can you specify what exactly you're looking for?",
                "It seems that I'm not trained to handle this specific query.Can you specify what exactly you're looking for?",
                "I'm sorry, but I haven't been trained on how to handle that specific query.Can you specify what exactly you're looking for?",
                "I'm afraid I don't have a prepared response for that question.Can you specify what exactly you're looking for?",
                "It seems I haven't been taught how to answer that question yet.Can you specify what exactly you're looking for?",
                "Hmm, it appears this is beyond my current training.Can you specify what exactly you're looking for?",
                "I'm sorry, I don't have information on that topic at the moment.Can you specify what exactly you're looking for?",
                "Oops! It looks like I'm not trained for this query.Can you specify what exactly you're looking for?",
                "Ah, I haven't been briefed on that.Can you specify what exactly you're looking for?",
                "Unfortunately, I haven't learned about that yet.Can you specify what exactly you're looking for?",
                "I'm still learning, and it seems I haven't covered that topic.Can you specify what exactly you're looking for?"
            };

            Random random = new Random();
            int randomIndex = random.Next(0, notTrainAns.Count);
            string selectedResponse = notTrainAns[randomIndex];

            return selectedResponse;
        }
    }
        
}