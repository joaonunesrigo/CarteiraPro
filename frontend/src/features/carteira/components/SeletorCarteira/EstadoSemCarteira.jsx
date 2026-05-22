import { Button } from '../../../../components/Button'
import { IconeCarteira, IconeMais } from '../../../../components/Icons'

export function EstadoSemCarteira({ onCriar }) {
  return (
    <section
      className="flex flex-col items-center gap-4 rounded-2xl border border-slate-800 bg-slate-900/50 px-6 py-12 text-center"
      role="status"
      aria-live="polite"
    >
      <div className="flex h-14 w-14 items-center justify-center rounded-2xl bg-violet-500/15 text-violet-300 ring-1 ring-inset ring-violet-500/30">
        <IconeCarteira className="h-7 w-7" />
      </div>

      <div className="max-w-md space-y-1">
        <h2 className="text-lg font-semibold text-white">Crie sua primeira carteira</h2>
        <p className="text-sm text-slate-400">
          Antes de adicionar ativos, crie pelo menos uma carteira para organizá-los por mercado, estratégia ou moeda.
        </p>
      </div>

      <Button type="button" onClick={onCriar} className="inline-flex items-center gap-2">
        <IconeMais />
        Criar carteira
      </Button>
    </section>
  )
}
