using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow
{
    public class QuestionChange
    {
        public QuestionChange(QuestionChangeType type, Question question, Question oldQuestion) {
            this.Type = type;
            this.Question = question;
            this.OldQuestion = oldQuestion;
        }

        public QuestionChangeType Type { get; private set; }
        public Question Question { get; private set; }
        public Question OldQuestion { get; private set; }
    }

    public enum QuestionChangeType
    {
        Added,
        Changed,
        Removed
    }
}
