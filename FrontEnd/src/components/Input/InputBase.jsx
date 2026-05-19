import { cn } from '../../utils/cn'
import { campo, campoInvalido } from '../styles/field.styles'

export function InputBase({ erro, className, ...props }) {
  return (
    <input
      className={cn(campo, erro && campoInvalido, className)}
      aria-invalid={erro ? true : undefined}
      {...props}
    />
  )
}
