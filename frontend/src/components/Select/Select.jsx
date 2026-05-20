import { mensagemErroCampo, rotuloCampo } from '../styles/field.styles'
import { SelectBase } from './SelectBase'

export function Select({ rotulo, erro, id, className, children, ...props }) {
  const campoId = id ?? props.name

  if (!rotulo) {
    return (
      <SelectBase id={campoId} erro={erro} className={className} {...props}>
        {children}
      </SelectBase>
    )
  }

  const erroId = campoId ? `${campoId}-erro` : undefined

  return (
    <div className="relative">
      <div className="flex flex-col gap-1.5">
        <label htmlFor={campoId} className={rotuloCampo}>
          {rotulo}
        </label>
        <SelectBase
          id={campoId}
          erro={erro}
          className={className}
          aria-describedby={erro ? erroId : undefined}
          {...props}
        >
          {children}
        </SelectBase>
      </div>
      {erro && (
        <p id={erroId} role="alert" className={mensagemErroCampo}>
          {erro}
        </p>
      )}
    </div>
  )
}
