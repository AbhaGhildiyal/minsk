﻿// See https://aka.ms/new-console-template for more information
while(true){
    Console.Write(">");
    var line = Console.ReadLine();
    if(string.IsNullOrWhiteSpace(line))
        return;
    
    var lexer = new Lexer(line);
    while (true)    
    {
        var token = lexer.NextToken();
        if (token.Kind == SyntaxKind.EndofFileToken)
            break;
        Console.Write($"{token.Kind}: '{token.Text}'");
        if (token.Value != null)
            Console.Write($"{ token.Value}");
        Console.WriteLine();
    }
}
enum SyntaxKind{
    NumberToken,
    WhiteSpaceToken,
    PlusToken,
    CloseParenthesisToken,
    OpenParenthesisToken,
    SlashToken,
    StarToken,
    MinusToken,
    BadToken,
    EndofFileToken
}
class SyntaxToken{
    public SyntaxToken(SyntaxKind kind, int position , string text, object ?value){
        Kind = kind;
        Position = position;
        Text = text;
        Value = value;
    }

    public SyntaxKind Kind { get; }
    public int Position { get; }
    public string Text { get; }
    public object? Value { get; }
}
class Lexer{
    private readonly string _text;
    private int _position;
    public Lexer(string text){
        _text = text;
    }

    //Current gives the current value of character
    private char Current{
        get{
            if(_position >= _text.Length)
                return '\0';
            return _text[_position];
        }
    }
    private void Next(){
        _position++;
    }
    public SyntaxToken NextToken(){
        // <numbers>
        // + - * / ( )
        // <whitespaces>

        if (_position >= _text.Length)
            return new SyntaxToken(SyntaxKind.EndofFileToken, _position,"\0",null);
        if (char.IsDigit(Current)){
            var start = _position;
            while (char.IsDigit(Current))
                Next();
            var length = _position - start;
            var text = _text.Substring(start,length);
            int.TryParse(text, out var value);
            return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
        }
        if(char.IsWhiteSpace(Current)){
            var start = _position;
            while (char.IsWhiteSpace(Current))
                Next();
            var length = _position - start;
            var text = _text.Substring(start,length);
            return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null);
        }
        if (Current == '+')
            return new SyntaxToken(SyntaxKind.PlusToken, _position++,"+", null);
        else if(Current == '-')
            return new SyntaxToken(SyntaxKind.MinusToken, _position++,"-", null);
        else if(Current == '*')
            return new SyntaxToken(SyntaxKind.StarToken, _position++,"*", null);
        else if(Current == '/')
            return new SyntaxToken(SyntaxKind.SlashToken, _position++,"/", null);
        else if(Current == '(')
            return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++,"(", null);
        else if(Current == ')')
            return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++,")", null);
        

        return new SyntaxToken(SyntaxKind.BadToken,_position++,_text.Substring(_position-1,1),null);
    }
}

class Parser{
    private readonly SyntaxToken[] _tokens;
    private int _position;
    public Parser(string text)
    {
        var tokens = new List<SyntaxToken>();
        var lexer = new Lexer(text);
        SyntaxToken token;
        do{
            token = lexer.NextToken();
            if (token.Kind != SyntaxKind.WhiteSpaceToken && 
                token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            
        }while (token.Kind != SyntaxKind.EndofFileToken);

        tokens = tokens.ToArray();
    }
}