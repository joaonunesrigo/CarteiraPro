import { cn } from '../../utils/cn'
import { cartao } from '../styles/field.styles'

export function Card({ titulo, children, className }) {
  return (
    <section className={cn(cartao, 'p-6', className)}>
      {titulo && <h2 className="mb-4 text-lg font-medium text-white">{titulo}</h2>}
      {children}
    </section>
  )
}
