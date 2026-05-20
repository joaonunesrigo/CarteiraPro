import { Button } from '../Button'
import { Select } from '../Select'
import { OPCOES_TAMANHO_PAGINA } from './usePaginacao'

export function Paginacao({
  paginaAtual,
  totalPaginas,
  totalItens,
  tamanhoPagina,
  irParaPagina,
  mudarTamanhoPagina,
  className = '',
}) {
  if (totalItens === 0) return null

  const primeiroItem = (paginaAtual - 1) * tamanhoPagina + 1
  const ultimoItem = Math.min(paginaAtual * tamanhoPagina, totalItens)

  return (
    <div
      className={`mt-4 flex flex-wrap items-center justify-between gap-3 text-sm text-slate-400 ${className}`}
    >
      <div className="flex items-center gap-2">
        <span>Itens por página:</span>
        <Select
          value={tamanhoPagina}
          onChange={(evento) => mudarTamanhoPagina(Number(evento.target.value))}
          className="min-w-[5rem]"
        >
          {OPCOES_TAMANHO_PAGINA.map((opcao) => (
            <option key={opcao} value={opcao}>
              {opcao}
            </option>
          ))}
        </Select>
      </div>

      <span className="tabular-nums">
        {primeiroItem}–{ultimoItem} de {totalItens}
      </span>

      <div className="flex items-center gap-2">
        <Button
          type="button"
          variante="secondary"
          disabled={paginaAtual === 1}
          onClick={() => irParaPagina(paginaAtual - 1)}
        >
          Anterior
        </Button>
        <span className="tabular-nums">
          {paginaAtual} / {totalPaginas}
        </span>
        <Button
          type="button"
          variante="secondary"
          disabled={paginaAtual === totalPaginas}
          onClick={() => irParaPagina(paginaAtual + 1)}
        >
          Próxima
        </Button>
      </div>
    </div>
  )
}
