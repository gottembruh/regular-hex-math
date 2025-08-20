using System;
using System.Text;
using RegularHexagonMath;

namespace RegularHexMath
{
    public class HexagonalException : Exception
    {
        private class MessageBuilder
        {
            private readonly StringBuilder _message = new();

            public MessageBuilder Append(string message)
            {
                _message.AppendLine(message);
                return this;
            }

            public MessageBuilder Append(HexagonalGrid grid)
            {
                Append(nameof(grid.Type), grid.Type);
                Append(nameof(grid.InnerRadius), grid.InnerRadius);
                Append(nameof(grid.OuterRadius), grid.OuterRadius);
                return this;
            }

            public MessageBuilder Append(params (string, object)[] fields)
            {
                foreach (var (paramName, paramValue) in fields)
                {
                    Append(paramName, paramValue);
                }

                return this;
            }

            private void Append(string paramName, object paramValue)
            {
                _message.Append($"{paramName}={paramValue}; ");
            }

            public override string ToString()
            {
                return _message.ToString();
            }
        }

        public HexagonalException(string message)
            : base(message)
        {
        }

        public HexagonalException(string message, params (string, object)[] fields)
            : base(CreateBuilder(message).Append(fields).ToString())
        {
        }

        public HexagonalException(string message, HexagonalGrid grid)
            : base(CreateBuilder(message).Append(grid).ToString())
        {
        }

        public HexagonalException(string message, HexagonalGrid grid, params (string, object)[] fields)
            : base(CreateBuilder(message).Append(grid).Append(fields).ToString())
        {
        }

        private static MessageBuilder CreateBuilder(string message)
        {
            return new MessageBuilder().Append(message);
        }
    }
}