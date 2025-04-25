let messages = [
    { role: "system", content: "You are a helpful assistant." }
];

async function sendMessage() {
    const input = document.getElementById("user-input");
    const chatBox = document.getElementById("chat-box");
    const userMessage = input.value.trim();
    if (!userMessage) return;

    messages.push({ role: "user", content: userMessage });
    chatBox.innerHTML += `<div class="message user"><strong>You:</strong> ${userMessage}</div>`;
    input.value = "";
    chatBox.scrollTop = chatBox.scrollHeight;

    const response = await fetch("/chat", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ messages }),
    });

    const data = await response.json();
    chatBox.innerHTML += `<div class="message Chatty"><strong>Chatty:</strong> ${data.reply}</div>`;
    messages.push({ role: "assistant", content: data.reply });
    chatBox.scrollTop = chatBox.scrollHeight;
}

async function uploadFile() {
    const fileInput = document.getElementById("fileInput");
    const file = fileInput.files[0];
    if (!file) return alert("Please select a file!");

    const formData = new FormData();
    formData.append("file", file);

    const response = await fetch("/upload", {
        method: "POST",
        body: formData
    });

    const result = await response.json();
    alert(result.message);

    
}