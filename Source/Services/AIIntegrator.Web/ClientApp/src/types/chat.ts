export interface ChatMessage {
    message: string;
    context?: any;
}

export interface ChatResponse {
    userMessage: string;
    response: string;
    function?: {
        name: string;
        parameters: any[];
    };
}