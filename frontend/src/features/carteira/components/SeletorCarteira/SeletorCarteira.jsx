import { Button } from '../../../../components/Button'
import { IconeCarteira, IconeEstrela, IconeLapis, IconeMais } from '../../../../components/Icons'
import { Select } from '../../../../components/Select'
import { obterSiglaMoeda } from '../../constants/moedas'

export function SeletorCarteira({ carteiras }) {
  const {
    carteiras: lista,
    carteiraAtiva,
    carteiraAtivaId,
    carregando,
    selecionarCarteira,
    abrirNovaCarteira,
    abrirEditarCarteira,
  } = carteiras

  const desabilitado = carregando || lista.length === 0

  return (
    <div className="flex flex-col gap-2 rounded-xl border border-slate-800 bg-slate-900/60 p-2 sm:flex-row sm:items-center sm:gap-1.5">
      <div className="hidden h-9 w-9 items-center justify-center rounded-lg bg-violet-500/10 text-violet-300 sm:flex">
        <IconeCarteira />
      </div>

      <div className="flex min-w-0 flex-1 items-center gap-2">
        <Select
          id="seletor-carteira-ativa"
          aria-label="Carteira ativa"
          value={carteiraAtivaId ?? ''}
          onChange={(evento) => selecionarCarteira(evento.target.value)}
          disabled={desabilitado}
          className="min-w-[200px] border-violet-500 bg-violet-600 text-sm font-medium text-white shadow-none hover:bg-violet-500 focus:border-violet-300 focus:ring-violet-300/40 sm:min-w-[220px]"
        >
          {lista.length === 0 && <option value="">Nenhuma carteira</option>}
          {lista.map((carteira) => (
            <option key={carteira.id} value={carteira.id}>
              {carteira.nome} · {obterSiglaMoeda(carteira.moeda)}
            </option>
          ))}
        </Select>

        {carteiraAtiva?.padrao && (
          <span
            className="inline-flex items-center gap-1 rounded-full border border-amber-400/30 bg-amber-400/10 px-2 py-0.5 text-xs font-medium text-amber-200"
            title="Carteira padrão"
          >
            <IconeEstrela />
            Padrão
          </span>
        )}
      </div>

      <div className="flex items-center gap-1.5">
        <Button
          type="button"
          variante="secondary"
          onClick={abrirEditarCarteira}
          disabled={!carteiraAtiva}
          aria-label="Editar carteira"
          title="Editar carteira"
          className="inline-flex items-center gap-1.5 px-3 py-2 text-sm"
        >
          <IconeLapis />
          <span className="hidden lg:inline">Editar</span>
        </Button>

        <Button
          type="button"
          variante="primary"
          onClick={abrirNovaCarteira}
          aria-label="Nova carteira"
          title="Nova carteira"
          className="inline-flex items-center gap-1.5 px-3 py-2 text-sm"
        >
          <IconeMais />
          <span className="hidden lg:inline">Nova</span>
        </Button>
      </div>
    </div>
  )
}
