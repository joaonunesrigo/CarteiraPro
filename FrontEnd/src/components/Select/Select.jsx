import { cn } from '../../utils/cn'
import { campo } from '../styles/field.styles'

export function Select({ className, children, ...props }) {
  return (
    <select className={cn(campo, className)} {...props}>
      {children}
    </select>
  )
}
