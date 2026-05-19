import { cn } from "../../utils/cn";
import { VARIANTES } from "./button.styles";

export function Button({
  variante = "primary",
  className,
  type = "button",
  children,
  ...props
}) {
  return (
    <button
      type={type}
      className={cn(VARIANTES[variante], className)}
      {...props}
    >
      {children}
    </button>
  );
}
