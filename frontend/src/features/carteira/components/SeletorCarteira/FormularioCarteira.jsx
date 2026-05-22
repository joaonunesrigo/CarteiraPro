import { Button } from '../../../../components/Button'
import { IconeLixeira } from '../../../../components/Icons'
import { Input } from '../../../../components/Input'
import { Select } from '../../../../components/Select'
import { MOEDAS } from '../../constants/moedas'

export function FormularioCarteira({
  modo = 'criar',
  formulario,
  erros,
  atualizarCampo,
  enviando,
  podeExcluir,
  aoSalvar,
  aoExcluir,
  aoCancelar,
}) {
  const ehEdicao = modo === 'editar'

  return (
    <form noValidate onSubmit={aoSalvar} className="flex flex-col gap-6">
      <div className="grid gap-x-4 gap-y-8 sm:grid-cols-2">
        <Input
          rotulo="Nome"
          id="carteira-nome"
          erro={erros?.nome}
          placeholder="Ex: Carteira BR"
          value={formulario.nome}
          onChange={atualizarCampo('nome')}
          autoFocus
        />

        <Select
          rotulo="Moeda"
          id="carteira-moeda"
          erro={erros?.moeda}
          value={formulario.moeda}
          onChange={atualizarCampo('moeda')}
          disabled={ehEdicao}
        >
          {MOEDAS.map((moeda) => (
            <option key={moeda.valor} value={moeda.valor}>
              {moeda.rotulo}
            </option>
          ))}
        </Select>

        <div className="sm:col-span-2">
          <Input
            rotulo="Descrição (opcional)"
            id="carteira-descricao"
            placeholder="Ex: Buy & hold de longo prazo"
            value={formulario.descricao ?? ''}
            onChange={atualizarCampo('descricao')}
          />
        </div>

        <label className="flex items-center gap-3 rounded-lg border border-slate-800 bg-slate-900/50 px-3 py-2.5 text-sm text-slate-200 sm:col-span-2">
          <input
            type="checkbox"
            checked={Boolean(formulario.padrao)}
            onChange={atualizarCampo('padrao')}
            className="h-4 w-4 rounded border-slate-600 bg-slate-900 text-violet-500 focus:ring-violet-500"
          />
          <span>
            Definir como <strong className="text-white">carteira padrão</strong>
            <span className="ml-2 text-xs text-slate-500">Selecionada automaticamente ao abrir o app</span>
          </span>
        </label>
      </div>

      <div className="flex flex-col-reverse items-stretch gap-3 border-t border-slate-800 pt-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          {ehEdicao && podeExcluir && (
            <Button
              type="button"
              variante="perigo"
              onClick={aoExcluir}
              disabled={enviando}
              className="inline-flex items-center gap-2"
            >
              <IconeLixeira />
              Excluir carteira
            </Button>
          )}
        </div>

        <div className="flex flex-col gap-3 sm:flex-row">
          <Button type="button" variante="secondary" onClick={aoCancelar} disabled={enviando}>
            Cancelar
          </Button>
          <Button type="submit" disabled={enviando}>
            {enviando ? 'Salvando...' : ehEdicao ? 'Salvar alterações' : 'Criar carteira'}
          </Button>
        </div>
      </div>
    </form>
  )
}
