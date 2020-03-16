CREATE DATABASE testeCervantes;

CREATE TABLE contatos (
	numero bigint not null primary key,
	nome character varying(150) not null
)

CREATE TABLE logs (
	dataehora timestamp not null,
	operacao character varying(20) not null
)

CREATE OR REPLACE FUNCTION add_contatos(_numero bigint, _nome character varying)
returns int as
$$
begin
	if (select count(*) from contatos where numero = _numero) > 0 then
		return 1; -- indica que o contato ja existe
	elsif _numero = 0 then 
		return 2; -- indica que o numero é igual a 0
	elsif LENGTH(_nome) = 0 then
		return 3; -- indica que o campo _texto veio vazio
	else
		insert into contatos values(_numero, _nome);
		return 0; -- indica que o contato foi criado
	end if;
end
$$
language plpgsql

--------- function que será chamada pela trigger para gerar o log salvando a
--------- operação realizada na tabela contatos

CREATE OR REPLACE FUNCTION gerar_log()
returns trigger as
$$
Begin

         insert into logs values (now(), TG_OP);

         return new;

end;
$$ 
language 'plpgsql';


----------  trigger que verifica a tabela contatos e quando um insert, update ou delete
----------	ocorre ele chama a function de gerar log

create trigger tr_gera_log after insert or update or delete on contatos
for each row execute procedure gerar_log();


---- function editar contato

CREATE OR REPLACE FUNCTION editar_contato(_numero_antigo bigint, _numero_novo bigint, _nome_antigo character varying, _nome_novo character varying)
returns int as
$$
begin
	if (select count(*) from contatos where numero = _numero_novo) > 0 then
		if (_nome_antigo = _nome_novo) then
			return 1; --- o numero já existe na agenda
		else
			update contatos
			set numero = _numero_novo, nome = _nome_novo
			where numero = _numero_antigo;
			return 0;
		end if;
	elsif _numero_novo = 0 then
		return 2; --- numero igual a 0
	elsif LENGTH(_nome_novo) = 0 then
		return 3; --- campo de texto vazio
	else 
		update contatos
		set numero = _numero_novo, nome = _nome_novo
		where numero = _numero_antigo;
		return 0;
	end if;
end
$$
language plpgsql;

---- function deletar contato

CREATE OR REPLACE FUNCTION deletar_contato(_numero bigint)
returns int as
$$
begin		
	if (select count(*) from contatos where numero = _numero) > 0 then
		delete from contatos where numero = _numero;
		return 0; --- numero deletado
	else
		return 1;
	end if;
end
$$
language plpgsql;
