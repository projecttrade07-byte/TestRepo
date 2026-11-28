const eventsList = document.querySelector("#events-list");
const listMessage = document.querySelector("#list-message");
const eventCount = document.querySelector("#event-count");

const dateFormatter = new Intl.DateTimeFormat(undefined, {
  weekday: "short",
  month: "short",
  day: "numeric"
});

const timeFormatter = new Intl.DateTimeFormat(undefined, {
  hour: "numeric",
  minute: "2-digit"
});

function makeElement(tag, className, text) {
  const element = document.createElement(tag);
  if (className) element.className = className;
  if (text) element.textContent = text;
  return element;
}

function renderEvent(event, index) {
  const card = makeElement("article", "event-card");
  const date = new Date(event.startsAt);
  const dateBlock = makeElement("div", "event-date");
  dateBlock.append(
    makeElement("span", "date-month", new Intl.DateTimeFormat(undefined, { month: "short" }).format(date)),
    makeElement("span", "date-day", new Intl.DateTimeFormat(undefined, { day: "2-digit" }).format(date))
  );

  const detail = makeElement("div", "event-detail");
  detail.append(
    makeElement("p", "event-kicker", `GATHERING ${String(index + 1).padStart(2, "0")}`),
    makeElement("h3", "event-title", event.title),
    makeElement("p", "event-location", event.location)
  );
  if (event.description) {
    detail.append(makeElement("p", "event-description", event.description));
  }

  const timing = makeElement("div", "event-timing");
  timing.append(
    makeElement("span", "time-label", "DATE & TIME"),
    makeElement("span", "event-time", `${dateFormatter.format(date)} · ${timeFormatter.format(date)} – ${timeFormatter.format(new Date(event.endsAt))}`)
  );

  card.append(dateBlock, detail, timing);
  return card;
}

async function loadEvents() {
  try {
    const response = await fetch("/api/events");
    if (!response.ok) {
      throw new Error("Could not load events. Please refresh to try again.");
    }

    const events = await response.json();
    listMessage.textContent = "";
    eventCount.textContent = `${events.length} ${events.length === 1 ? "event" : "events"}`;

    if (events.length === 0) {
      listMessage.textContent = "Nothing on the calendar just yet. Add your first event to get started.";
      listMessage.classList.add("empty-state");
      return;
    }

    events.forEach((event, index) => eventsList.append(renderEvent(event, index)));
  } catch (error) {
    listMessage.textContent = error.message || "Could not load events. Please refresh to try again.";
    listMessage.classList.add("error");
  }
}

loadEvents();
