import { render, screen } from "@testing-library/react";
import App from "./App";

test("renders dashboard title", () => {
  render(<App />);
  expect(screen.getByText(/smart home telemetry/i)).toBeInTheDocument();
});
