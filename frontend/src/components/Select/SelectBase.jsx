import { cn } from '../../utils/cn'
import { campo, campoInvalido } from '../styles/field.styles'

export function SelectBase({ erro, className, children, ...props }) {
  return (
    <select
      className={cn(campo, erro && campoInvalido, className)}
      aria-invalid={erro ? true : undefined}
      {...props}
    >
      {children}
    </select>
  )
}
