import { cn } from '../../utils/cn'

export function Tabs({ abas, abaAtiva, onMudarAba, paineis }) {
  return (
    <div>
      <div role="tablist" aria-label="Navegação do dashboard" className="flex gap-1 rounded-lg border border-slate-800 bg-slate-900/50 p-1">
        {abas.map((aba) => {
          const ativa = abaAtiva === aba.id
          return (
            <button
              key={aba.id}
              type="button"
              role="tab"
              aria-selected={ativa}
              aria-controls={`painel-${aba.id}`}
              id={`aba-${aba.id}`}
              onClick={() => onMudarAba(aba.id)}
              className={cn(
                'flex-1 rounded-md px-4 py-2 text-sm font-medium transition',
                ativa ? 'bg-violet-600 text-white shadow-sm' : 'text-slate-400 hover:bg-slate-800 hover:text-slate-200',
              )}
            >
              {aba.rotulo}
            </button>
          )
        })}
      </div>

      {abas.map((aba) => {
        const ativa = abaAtiva === aba.id
        return (
          <div
            key={aba.id}
            id={`painel-${aba.id}`}
            role="tabpanel"
            aria-labelledby={`aba-${aba.id}`}
            hidden={!ativa}
            className={cn('mt-6 space-y-8', !ativa && 'hidden')}
          >
            {paineis[aba.id]}
          </div>
        )
      })}
    </div>
  )
}
