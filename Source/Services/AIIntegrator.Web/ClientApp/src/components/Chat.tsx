import { useState, useRef, useEffect } from 'react';
import './Chat.css';
import config from '../config';

interface Message {
    text: string;
    isUser: boolean;
}

export const Chat = () => {
    const [messages, setMessages] = useState<Message[]>([]);
    const [inputText, setInputText] = useState('');
    const messagesEndRef = useRef<HTMLDivElement>(null);

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    };

    useEffect(() => {
        scrollToBottom();
    }, [messages]);

    const sendMessage = async () => {
        if (!inputText.trim()) return;

        const userMessage: Message = {
            text: inputText,
            isUser: true
        };

        setMessages(prev => [...prev, userMessage]);
        setInputText('');

        try {
            const response = await fetch(`${config.apiBaseUrl}api/chat/send-message`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    system: "HOSP",
                    message: {
                        message: inputText
                    }
                })
            });

            if (response.ok) {
                const data = await response.json();
                const botMessage: Message = {
                    text: data.response,
                    isUser: false
                };
                setMessages(prev => [...prev, botMessage]);
            }
        } catch (error) {
            console.error('Error sending message:', error);
        }
    };

    return (
        <div className="chat-container">
            <div className="messages-container">
                {messages.map((msg, index) => (
                    <div
                        key={index}
                        className={`message ${msg.isUser ? 'user-message' : 'bot-message'}`}
                    >
                        {msg.text}
                    </div>
                ))}
                <div ref={messagesEndRef} />
            </div>
            <div className="input-container">
                <input
                    type="text"
                    value={inputText}
                    onChange={(e) => setInputText(e.target.value)}
                    onKeyPress={(e) => e.key === 'Enter' && sendMessage()}
                    placeholder="Wpisz wiadomość..."
                />
                <button onClick={sendMessage}>Wyślij</button>
            </div>
        </div>
    );
};
