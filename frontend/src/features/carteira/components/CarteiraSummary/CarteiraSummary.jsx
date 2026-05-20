import { StatCard } from '../../../../components/StatCard'

export function CarteiraSummary({ cartoes }) {
  if (!cartoes.length) return null

  return (
    <section className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
      {cartoes.map((cartao) => (
        <StatCard
          key={cartao.id}
          rotulo={cartao.rotulo}
          valor={cartao.valorFormatado}
          comSinal={cartao.comSinal}
          positivo={cartao.positivo}
        />
      ))}
    </section>
  )
}
