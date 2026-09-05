const WebSocket = globalThis.WebSocket;
process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";

async function testSignalR() {
  const ws1 = new WebSocket("wss://localhost:5001/hubs/tms?studentId=1");
  const ws2 = new WebSocket("wss://localhost:5001/hubs/tms?studentId=2");

  let student1Received = false;
  let student2Received = false;

  await Promise.all([
    new Promise(res => ws1.onopen = () => {
      ws1.send(JSON.stringify({ protocol: "json", version: 1 }) + "\x1e");
      res();
    }),
    new Promise(res => ws2.onopen = () => {
      ws2.send(JSON.stringify({ protocol: "json", version: 1 }) + "\x1e");
      res();
    })
  ]);

  ws1.onmessage = (event) => {
    const messages = event.data.split("\x1e").filter(Boolean);
    for (const msg of messages) {
      const data = JSON.parse(msg);
      if (data.target === "ReceiveTranscriptReady") {
        console.log("Student 1 received:", data.arguments);
        student1Received = true;
      }
    }
  };

  ws2.onmessage = (event) => {
    const messages = event.data.split("\x1e").filter(Boolean);
    for (const msg of messages) {
      const data = JSON.parse(msg);
      if (data.target === "ReceiveTranscriptReady") {
        console.log("Student 2 received:", data.arguments);
        student2Received = true;
      }
    }
  };

  console.log("Both students connected to TmsHub.");

  // Trigger transcript for student 1
  const postRes = await fetch("https://localhost:5001/api/v2/transcripts", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ studentId: 1 })
  });
  const postData = await postRes.json();
  console.log("POST /api/v2/transcripts returned 202 for reportId:", postData.reportId);

  // Wait 7 seconds for the worker (5s duration) to complete
  await new Promise(res => setTimeout(res, 7000));

  ws1.close();
  ws2.close();

  console.log("SUMMARY: student1Received =", student1Received, ", student2Received =", student2Received);
  if (student1Received && !student2Received) {
    console.log("SIGNALR VERIFICATION PASSED: Only student 1 received notification!");
  } else {
    console.error("SIGNALR VERIFICATION FAILED!");
    process.exit(1);
  }
}

testSignalR().catch(err => {
  console.error("Error:", err);
  process.exit(1);
});
