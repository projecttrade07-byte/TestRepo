const form = document.querySelector("#event-form");
const message = document.querySelector("#form-message");

form.addEventListener("submit", async (event) => {
  event.preventDefault();
  message.textContent = "";
  message.className = "form-message";

  const formData = new FormData(form);
  const eventDetails = Object.fromEntries(formData.entries());

  const submitButton = form.querySelector('button[type="submit"]');
  submitButton.disabled = true;
  submitButton.textContent = "Saving…";

  try {
    const response = await fetch("/api/events", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(eventDetails)
    });

    if (!response.ok) {
      const problem = await response.json();
      const validationErrors = Object.values(problem.errors || {}).flat();
      throw new Error(validationErrors.join(" ") || problem.title || "Unable to save the event.");
    }

    window.location.assign("/events.html");
  } catch (error) {
    message.textContent = error.message || "Unable to save the event. Please try again.";
    message.classList.add("error");
  } finally {
    submitButton.disabled = false;
    submitButton.innerHTML = 'Save event <span aria-hidden="true">↗</span>';
  }
});
