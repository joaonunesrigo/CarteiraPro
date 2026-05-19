import { cn } from '../../utils/cn'
import { campo } from '../styles/field.styles'

export function Input({ className, ...props }) {
  return <input className={cn(campo, className)} {...props} />
}
